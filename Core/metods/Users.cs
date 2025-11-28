public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string password { get; set; }
}

public class Music
{
    public int id { get; set; }
    public string name { get; set; }
    public string artist { get; set; }
    public string song_LONGELOS { get; set; }
}

public class UserPlay
{
    public int user_id { get; set; }
    public int music_id { get; set; }
}   