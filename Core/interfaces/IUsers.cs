using Microsoft.Extensions.Configuration;
using MySqlConnector;
using models;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<int> AddUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}

public interface IMusicService
{
    Task<List<MusicDto>> GetAllMusicAsync();
    Task<Music> GetMusicByIdAsync(int id);
    Task<Music> GetMusicAudioAsync(int id);
    Task<int> AddMusicAsync(Music music);
    Task<bool> DeleteMusicAsync(int id);
    
}