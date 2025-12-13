using frontend.Components;

var builders = WebApplication.CreateBuilder(args);

var builder = WebApplication.CreateBuilder(args);

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