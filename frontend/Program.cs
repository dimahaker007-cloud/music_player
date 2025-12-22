using frontend.Components;
using Polly; // Для основних класів Polly (наприклад, Policy)
using Polly.Extensions.Http; // Для HttpPolicyExtensions (HandleTransientHttpError)

var builders = WebApplication.CreateBuilder(args);

var builder = WebApplication.CreateBuilder(args);
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() // 5xx, 408
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // 429
    .WaitAndRetryAsync(3, retryAttempt => 
    {
        // Експоненційна затримка (2, 4, 8 сек) + Jitter (рандомні мс)
        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) 
               + TimeSpan.FromMilliseconds(new Random().Next(0, 100));
    });

builder.Services.AddHttpClient("ApiClient", client => {
        client.BaseAddress = new Uri("https://localhost:44302/");
    })
    .AddPolicyHandler(retryPolicy);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient("LocalApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:44302/"); // Ваш URL
});

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:44302/")
});

builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMusicService, MusicService>();
builder.Services.AddScoped<IMusicPlayerService, MusicPlayerService>();
builder.Services.AddSingleton<AuthStateService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers(); // Для API контролерів

// Для Blazor Web App використовуйте MapRazorComponents
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // або AddInteractiveWebAssemblyRenderMode()

app.Run();