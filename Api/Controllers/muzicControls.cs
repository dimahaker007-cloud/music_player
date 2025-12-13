// Controllers/MusicPlayerController.cs
using Microsoft.AspNetCore.Mvc;
using models;

[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase
{
    private readonly IMusicService _musicService;
    private readonly ILogger<MusicController> _logger;

    public MusicController(
        IMusicService musicService,
        ILogger<MusicController> logger)
    {
        _musicService = musicService;
        _logger = logger;
    }
    
    // GET: api/music
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MusicDto>>> GetAll()
    {
        try
        {
            return Ok(await _musicService.GetAllMusicAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні пісень");
            return StatusCode(500, "Внутрішня помилка сервера");
        }
    }
    
    // GET: api/music/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<MusicDto>> GetById(int id)
    {
        try
        {
            var music = await _musicService.GetMusicByIdAsync(id);
            return music != null ? Ok(music) : NotFound($"Пісня з ID {id} не знайдена");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні пісні ID: {Id}", id);
            return StatusCode(500, "Внутрішня помилка сервера");
        }
    }
    
    // GET: api/music/stream/{id}
    [HttpGet("stream/{id}")]
    public async Task<IActionResult> StreamMusic(int id)
    {
        try
        {
            var music = await _musicService.GetMusicAudioAsync(id);
            
            if (music == null || music.Song == null || music.Song.Length == 0)
            {
                return NotFound($"Музика з ID {id} не знайдена");
            }

            return File(music.Song, "audio/mpeg", enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при отриманні аудіо: {ex.Message}");
            return StatusCode(500, "Внутрішня помилка сервера");
        }
    }
    
    // POST: api/music
    [HttpPost]
    public async Task<ActionResult<Music>> Create([FromBody] Music music)
    {
        try
        {
            var id = await _musicService.AddMusicAsync(music);
            return CreatedAtAction(nameof(GetById), new { id }, music);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при додаванні пісні");
            return BadRequest($"Не вдалося додати пісню: {ex.Message}");
        }
    }
    
    // DELETE: api/music/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _musicService.DeleteMusicAsync(id);
            return deleted ? NoContent() : NotFound($"Пісня з ID {id} не знайдена");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при видаленні пісні ID: {Id}", id);
            return StatusCode(500, "Внутрішня помилка сервера");
        }
    }
}