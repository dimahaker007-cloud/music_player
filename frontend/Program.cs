using frontend.Components;

var builders = WebApplication.CreateBuilder(args);

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

// Додайте сервіси для Blazor Web App
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // або AddInteractiveWebAssemblyComponents()
builder.Services.AddHttpClient("LocalApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:44302/"); // Ваш URL
});

// Або зареєструйте типовий HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:44302/")
});
// Додати HttpClient з правильним BaseAddress

// Додайте сервіси для API
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
// Додайте HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

// Конфігурація middleware
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