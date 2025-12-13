using models;

public interface IMusicService2
{
    public interface IMusicService
    {
        Task<List<MusicDto>> GetAllMusicAsync();
        Task<Music> GetMusicByIdAsync(int id);
        Task<Music> GetMusicAudioAsync(int id);
        Task<int> AddMusicAsync(Music music);
        Task<bool> DeleteMusicAsync(int id);
    
    }
}