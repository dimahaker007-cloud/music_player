namespace models
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
    }

    public class Music
    {
        public int Id { get; set; }  
        public string? Name { get; set; } = string.Empty; 
        public string? Artist { get; set; } = string.Empty;  
        public byte[] Song { get; set; } = Array.Empty<byte>(); 
    }

    public class UserPlay
    {
        public int user_id { get; set; }
        public int music_id { get; set; }
    }
}