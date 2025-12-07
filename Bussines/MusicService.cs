using Microsoft.Extensions.Configuration;
using MySqlConnector;
using models;
public class MusicService : IMusicService
{
    private readonly string _connectionString;

    public MusicService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<MusicDto>> GetAllMusicAsync()
    {
        var musicList = new List<MusicDto>();
        
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "SELECT id, name, artist FROM music";
            
            using (var command = new MySqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var music = new MusicDto()
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.IsDBNull(reader.GetOrdinal("name")) 
                            ? null 
                            : reader.GetString("name"),
                        Artist = reader.IsDBNull(reader.GetOrdinal("artist")) 
                            ? null 
                            : reader.GetString("artist")
                    };
                    musicList.Add(music);
                }
            }
        }
        
        return musicList;
    }

    public async Task<Music> GetMusicByIdAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "SELECT id, name, artist FROM music WHERE id = @Id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Music
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.IsDBNull(reader.GetOrdinal("name")) 
                                ? null 
                                : reader.GetString("name"),
                            Artist = reader.IsDBNull(reader.GetOrdinal("artist")) 
                                ? null 
                                : reader.GetString("artist")
                        };
                    }
                }
            }
        }
        
        return null;
    }

    public async Task<Music> GetMusicAudioAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "SELECT song FROM music WHERE id = @Id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Music
                        {
                            Song = (byte[])reader["song"]
                        };
                    }
                }
            }
        }
        
        return null;
    }

    public async Task<int> AddMusicAsync(Music music)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = @"INSERT INTO music (name, artist, song) 
                         VALUES (@Name, @Artist, @Song);
                         SELECT LAST_INSERT_ID();";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", music.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Artist", music.Artist ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Song", music.Song);
                
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
    }

    public async Task<bool> DeleteMusicAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var query = "DELETE FROM music WHERE id = @Id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
    }
}
