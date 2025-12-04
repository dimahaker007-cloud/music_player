using Microsoft.Extensions.Configuration;
using MySqlConnector;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
}

public interface IMusicService
{
    Task<List<Music>> GetAllMusicAsync();
    Task<Music> GetMusicByIdAsync(int id);
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
public class MusicService : IMusicService
{
    
    private readonly string _connectionString;

    public MusicService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<Music>> GetAllMusicAsync()
    {
        var musicList = new List<Music>();
        
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "SELECT id, name, artist FROM music";
            
            using (var command = new MySqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var music = new Music
                    {
                        id = reader.GetInt32("id"),
                        name = reader.IsDBNull(reader.GetOrdinal("name")) 
                            ? null 
                            : reader.GetString("name"),
                        artist = reader.IsDBNull(reader.GetOrdinal("artist")) 
                            ? null 
                            : reader.GetString("artist")
                    };
                    musicList.Add(music);
                }
            }
        }
        
        return musicList;
    }

    public Task<Music> GetMusicByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Music> GetMusicAudioAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "SELECT audio_data, content_type FROM music WHERE id = @Id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Music
                        {
                            audio_data = (byte[])reader["audio_data"],
                            content_type = reader.GetString("content_type")
                        };
                    }
                }
            }
        }
        
        return null;
    }
}