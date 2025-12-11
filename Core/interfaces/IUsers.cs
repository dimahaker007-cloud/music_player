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
    public async Task<int> AddUserAsync(User user)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "INSERT INTO users (name, password) VALUES (@Name, @Password); SELECT LAST_INSERT_ID();";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", user.name);
                command.Parameters.AddWithValue("@Password", user.password);
                
                // Виконуємо запит та отримуємо ID нового користувача
                var result = await command.ExecuteScalarAsync();
                
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                
                return 0; // Повертаємо 0 якщо щось пішло не так
            }
        }
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "UPDATE users SET name = @Name, password = @Password WHERE id = @Id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", user.id);
                command.Parameters.AddWithValue("@Name", user.name);
                command.Parameters.AddWithValue("@Password", user.password);
                
                var rowsAffected = await command.ExecuteNonQueryAsync();
                
                return rowsAffected > 0;
            }
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "DELETE FROM users WHERE id = @Id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                
                var rowsAffected = await command.ExecuteNonQueryAsync();
                
                return rowsAffected > 0;
            }
        }
    }
}