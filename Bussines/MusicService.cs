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
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "SELECT id, name, artist FROM music";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        
        var musicList = new List<MusicDto>();
        
        while (await reader.ReadAsync())
        {
            musicList.Add(new MusicDto
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetStringOrNull("name"),
                Artist = reader.GetStringOrNull("artist")
            });
        }
        
        return musicList;
    }

    public async Task<Music> GetMusicByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "SELECT id, name, artist FROM music WHERE id = @Id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        
        return await reader.ReadAsync() ? new Music
        {
            Id = reader.GetInt32("id"),
            Name = reader.GetStringOrNull("name"),
            Artist = reader.GetStringOrNull("artist")
        } : null;
    }

    public async Task<Music> GetMusicAudioAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "SELECT song FROM music WHERE id = @Id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return new Music
            {
                Id = id,
                Song = reader["song"] as byte[] ?? Array.Empty<byte>()
            };
        }
        
        return null;
    }

    public async Task<int> AddMusicAsync(Music music)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = @"INSERT INTO music (name, artist, song) 
                     VALUES (@Name, @Artist, @Song);
                     SELECT LAST_INSERT_ID();";
        
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", music.Name ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Artist", music.Artist ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Song", music.Song);
        
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<bool> DeleteMusicAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "DELETE FROM music WHERE id = @Id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        
        return await command.ExecuteNonQueryAsync() > 0;
    }
}

// Допоміжний extension метод для спрощення
public static class DataReaderExtensions
{
    public static string? GetStringOrNull(this MySqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}