// Services/Concrete/MusicPlayerService.cs

using Microsoft.Extensions.Logging;
public interface IMusicPlayerService
{
    int CurrentTrackId { get; }
    bool IsPlaying { get; }
    double CurrentTime { get; }
    double Duration { get; }
    double Volume { get; set; }
    
    event Action OnPlaybackChanged;
    
    Task PlayAsync(int musicId);
    Task PauseAsync();
    Task StopAsync();
    Task SeekAsync(double seconds);
    Task<byte[]> GetAudioDataAsync(int musicId);
    Task<Stream> GetAudioStreamAsync(int musicId);
    string GetCurrentTimeFormatted();
    string GetDurationFormatted();
    double GetProgressPercentage();
    Task NextAsync();
    Task PreviousAsync();
}
public class MusicPlayerService : IMusicPlayerService, IDisposable
{
    private readonly IMusicService _musicService;
    private readonly ILogger<MusicPlayerService> _logger;
    private System.Timers.Timer _playbackTimer;
    private MemoryStream _currentAudioStream;
    private bool _disposed = false;

    public int CurrentTrackId { get; private set; }
    public bool IsPlaying { get; private set; }
    public double CurrentTime { get; private set; }
    public double Duration { get; private set; }
    public double Volume { get; set; } = 0.8;

    public event Action OnPlaybackChanged;

    public MusicPlayerService(IMusicService musicService, ILogger<MusicPlayerService> logger)
    {
        _musicService = musicService;
        _logger = logger;
        
        InitializePlaybackTimer();
    }

    private void InitializePlaybackTimer()
    {
        _playbackTimer = new System.Timers.Timer(100); // Оновлення кожні 100 мс
        _playbackTimer.Elapsed += (sender, e) =>
        {
            if (IsPlaying && CurrentTime < Duration)
            {
                CurrentTime += 0.1; // Збільшуємо час на 100 мс
                OnPlaybackChanged?.Invoke();
            }
            else if (IsPlaying && CurrentTime >= Duration)
            {
                StopAsync().Wait();
            }
        };
        _playbackTimer.AutoReset = true;
    }

    public async Task PlayAsync(int musicId)
    {
        try
        {
            // Зупиняємо поточне відтворення
            await StopAsync();

            // Отримуємо новий трек
            var music = await _musicService.GetMusicAudioAsync(musicId);
            if (music?.Song == null || music.Song.Length == 0)
            {
                _logger.LogWarning("Не вдалося знайти аудіо для треку ID: {MusicId}", musicId);
                return;
            }

            CurrentTrackId = musicId;
            _currentAudioStream = new MemoryStream(music.Song);
            
            // Розрахунок тривалості (спрощений для MP3)
            Duration = CalculateDuration(music.Song);
            CurrentTime = 0;
            IsPlaying = true;
            
            _playbackTimer.Start();
            
            _logger.LogInformation("Почато відтворення треку ID: {MusicId}", musicId);
            OnPlaybackChanged?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при відтворенні треку ID: {MusicId}", musicId);
            throw;
        }
    }

    public Task PauseAsync()
    {
        if (IsPlaying)
        {
            IsPlaying = false;
            _playbackTimer.Stop();
            
            _logger.LogInformation("Пауза треку ID: {MusicId}", CurrentTrackId);
            OnPlaybackChanged?.Invoke();
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        IsPlaying = false;
        CurrentTime = 0;
        _playbackTimer.Stop();
        
        if (_currentAudioStream != null)
        {
            _currentAudioStream.Dispose();
            _currentAudioStream = null;
        }
        
        _logger.LogInformation("Зупинено відтворення треку ID: {MusicId}", CurrentTrackId);
        OnPlaybackChanged?.Invoke();
        
        return Task.CompletedTask;
    }

    public Task SeekAsync(double seconds)
    {
        if (seconds < 0) seconds = 0;
        if (seconds > Duration) seconds = Duration;
        
        CurrentTime = seconds;
        
        _logger.LogDebug("Перехід до {Seconds} секунд треку ID: {MusicId}", seconds, CurrentTrackId);
        OnPlaybackChanged?.Invoke();
        
        return Task.CompletedTask;
    }

    public async Task<byte[]> GetAudioDataAsync(int musicId)
    {
        var music = await _musicService.GetMusicAudioAsync(musicId);
        return music?.Song ?? Array.Empty<byte>();
    }

    public async Task<Stream> GetAudioStreamAsync(int musicId)
    {
        var music = await _musicService.GetMusicAudioAsync(musicId);
        if (music?.Song == null || music.Song.Length == 0)
            return Stream.Null;
            
        return new MemoryStream(music.Song);
    }

    // Додаткові методи контролю
    public Task NextAsync()
    {
        // Тут можна реалізувати логіку переходу до наступного треку
        _logger.LogInformation("Наступний трек");
        return Task.CompletedTask;
    }

    public Task PreviousAsync()
    {
        // Тут можна реалізувати логіку переходу до попереднього треку
        _logger.LogInformation("Попередній трек");
        return Task.CompletedTask;
    }

    public string GetCurrentTimeFormatted()
    {
        var timeSpan = TimeSpan.FromSeconds(CurrentTime);
        return $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:00}";
    }

    public string GetDurationFormatted()
    {
        var timeSpan = TimeSpan.FromSeconds(Duration);
        return $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:00}";
    }

    public double GetProgressPercentage()
    {
        if (Duration <= 0) return 0;
        return (CurrentTime / Duration) * 100;
    }

    // Допоміжні методи
    private double CalculateDuration(byte[] audioData)
    {
        if (audioData.Length == 0) return 0;
        
        // Приблизний розрахунок тривалості MP3
        // 128 kbps = 16 KB/s
        double bytesPerSecond = 128.0 * 1024 / 8;
        return audioData.Length / bytesPerSecond;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _playbackTimer?.Stop();
                _playbackTimer?.Dispose();
                _currentAudioStream?.Dispose();
            }
            _disposed = true;
        }
    }

    ~MusicPlayerService()
    {
        Dispose(false);
    }
}