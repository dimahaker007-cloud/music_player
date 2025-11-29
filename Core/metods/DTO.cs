public class UserDto
{
    public int id { get; set; }
    public string name { get; set; }
}

public class MusicDto
{
    public int id { get; set; }
    public string name { get; set; }
    public string artist { get; set; }
    public string song_LONGELOS { get; set; }
}

public class UserLoginDto
{
    public string name { get; set; }
    public string password { get; set; }
}