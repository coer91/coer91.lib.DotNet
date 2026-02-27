namespace coer91.NET
{
    public class HttpRequestDTO
    {
        public string Project { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
        public string User { get; set; }
        public string Role { get; set; }
        public int UtcOffset { get; set; } = 0;
        public DateTime? JWTExpiration { get; set; } 
    }
} 