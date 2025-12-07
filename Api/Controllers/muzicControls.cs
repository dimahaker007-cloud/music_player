// Controllers/MusicPlayerController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using models;

[ApiController]
[Route("api/[controller]")]
public class MusicPlayerController : ControllerBase
{
    private readonly IMusicPlayerService _playerService;
    private readonly IMusicService _musicService;
    private readonly ILogger<MusicPlayerController> _logger;

    public MusicPlayerController(
        IMusicPlayerService playerService,
        IMusicService musicService,
        ILogger<MusicPlayerController> logger)
    {
        _playerService = playerService;
        _musicService = musicService;
        _logger = logger;
    }
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<MusicDto>>> GetAllMusic()
    {
        try
        {
            var musicList = await _musicService.GetAllMusicAsync();
            return Ok(musicList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні всіх пісень");
            return StatusCode(500, "Внутрішня помилка сервера");
        }
    }
    // GET: api/musicplayer/play/{id}
    [HttpPost("play/{id}")]
    public async Task<IActionResult> Play(int id)
    {
        try
        {
            await _playerService.PlayAsync(id);
            return Ok(new 
            { 
                Message = "Відтворення почато", 
                TrackId = id,
                IsPlaying = _playerService.IsPlaying
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при запуску відтворення треку ID: {Id}", id);
            return BadRequest($"Не вдалося відтворити трек: {ex.Message}");
        }
    }

    // POST: api/musicplayer/pause
    [HttpPost("pause")]
    public IActionResult Pause()
    {
        try
        {
            _playerService.PauseAsync().Wait();
            return Ok(new 
            { 
                Message = "Відтворення призупинено",
                IsPlaying = _playerService.IsPlaying
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при паузі відтворення");
            return BadRequest($"Не вдалося поставити на паузу: {ex.Message}");
        }
    }

    // POST: api/musicplayer/stop
    [HttpPost("stop")]
    public IActionResult Stop()
    {
        try
        {
            _playerService.StopAsync().Wait();
            return Ok(new 
            { 
                Message = "Відтворення зупинено",
                IsPlaying = _playerService.IsPlaying,
                CurrentTime = 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при зупинці відтворення");
            return BadRequest($"Не вдалося зупинити відтворення: {ex.Message}");
        }
    }

    // POST: api/musicplayer/seek/{seconds}
    [HttpPost("seek/{seconds}")]
    public IActionResult Seek(double seconds)
    {
        try
        {
            _playerService.SeekAsync(seconds).Wait();
            return Ok(new 
            { 
                Message = $"Перехід до {seconds} секунд",
                CurrentTime = _playerService.CurrentTime
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при переході до позиції: {Seconds}", seconds);
            return BadRequest($"Не вдалося перейти до позиції: {ex.Message}");
        }
    }

    // GET: api/musicplayer/status
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            CurrentTrackId = _playerService.CurrentTrackId,
            IsPlaying = _playerService.IsPlaying,
            CurrentTime = _playerService.CurrentTime,
            Duration = _playerService.Duration,
            Volume = _playerService.Volume,
            CurrentTimeFormatted = _playerService.GetCurrentTimeFormatted(),
            DurationFormatted = _playerService.GetDurationFormatted(),
            ProgressPercentage = _playerService.GetProgressPercentage()
        });
    }

    // GET: api/musicplayer/stream/{id}
    [HttpGet("stream/{id}")]
    public async Task<IActionResult> StreamAudio(int id)
    {
        try
        {
            var music = await _musicService.GetMusicAudioAsync(id);
            if (music?.Song == null || music.Song.Length == 0)
                return NotFound($"Аудіо для треку ID {id} не знайдено");

            var stream = new MemoryStream(music.Song);
            
            // Визначаємо тип контенту
            // Можна додати логіку для визначення типу файлу
            var contentType = "audio/mpeg";
            
            // Для стримінгу
            Response.Headers.Add("Accept-Ranges", "bytes");
            
            return File(stream, contentType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при стримінгуванні треку ID: {Id}", id);
            return StatusCode(500, "Внутрішня помилка сервера");
        }
    }

    // GET: api/musicplayer/audio/{id}
    [HttpGet("audio/{id}")]
    public async Task<IActionResult> GetAudio(int id)
    {
        try
        {
            var audioData = await _playerService.GetAudioDataAsync(id);
            if (audioData == null || audioData.Length == 0)
                return NotFound($"Аудіо для треку ID {id} не знайдено");

            return File(audioData, "audio/mpeg");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні аудіо треку ID: {Id}", id);
            return StatusCode(500, "Внутрішня помилка сервера");
        }
    }

    // POST: api/musicplayer/volume/{level}
    [HttpPost("volume/{level}")]
    public IActionResult SetVolume(double level)
    {
        try
        {
            if (level < 0) level = 0;
            if (level > 1) level = 1;
            
            _playerService.Volume = level;
            
            return Ok(new 
            { 
                Message = $"Гучність встановлено на {level:P0}",
                Volume = _playerService.Volume
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при зміні гучності на рівень: {Level}", level);
            return BadRequest($"Не вдалося змінити гучність: {ex.Message}");
        }
    }
}