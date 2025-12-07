using Microsoft.Extensions.Configuration;
using MySqlConnector;
using models;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
}

public interface IMusicService
{
    Task<List<MusicDto>> GetAllMusicAsync();
    Task<Music> GetMusicByIdAsync(int id);
    Task<Music> GetMusicAudioAsync(int id);
    Task<int> AddMusicAsync(Music music);
    Task<bool> DeleteMusicAsync(int id);
    
}

public class UserService : IUserService
{
    private readonly string _connectionString;

    public UserService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = new List<User>();
        
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "SELECT id, name, password FROM users";
            
            using (var command = new MySqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var user = new User
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        password = reader.GetString("password")
                    };
                    users.Add(user);
                }
            }
        }
        
        return users;
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "SELECT id, name, password FROM users WHERE id = @Id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            password = reader.GetString("password")
                        };
                    }
                }
            }
        }
        
        return null;
    }
}