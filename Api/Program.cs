
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Додаємо сервіси
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Додаємо CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Реєструємо сервіси
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMusicService, MusicService>();
builder.Services.AddScoped<IMusicPlayerService, MusicPlayerService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// 1. Middleware для X-Request-Id
/*app.Use(async (context, next) => {
    var requestId = context.Request.Headers["X-Request-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
    context.Response.Headers["X-Request-Id"] = requestId;
    context.Items["RequestId"] = requestId; // Зберігаємо для логів/помилок
    await next();
});*/   

// 2. Глобальна обробка помилок (Єдиний формат)

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerPathFeature?.Error; // Отримуємо реальну помилку

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new 
        {
            error = "Internal Server Error",
            // ТУТ МАЄ БУТИ exception.Message, щоб ви побачили текст помилки в браузері/Postman
            details = exception?.Message, 
            requestId = context.Items["RequestId"]?.ToString()
        });
    });
});
// Використовуємо CORS
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();