using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Check(CancellationToken ct)
    {
        // Якщо клієнт (AbortController) обірве запит через 1с, 
        // сервер отримає сигнал через CancellationToken 'ct'
        await Task.Delay(500, ct); // Імітація роботи
        return Ok(new { status = "ok" });
    }
}
