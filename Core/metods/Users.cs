public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string password { get; set; }
}

public class Music
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string artist { get; set; } = string.Empty;
    public byte[] audio_data { get; set; } = Array.Empty<byte>();
    public string content_type { get; set; } = "audio/mpeg";
}

public class UserPlay
{
    public int user_id { get; set; }
    public int music_id { get; set; }
}   