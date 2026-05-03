namespace coer91.NET
{
    public class HttpRequestDTO
    {
        public string Project { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public int PartnerId { get; set; }
        public string Partner { get; set; }
        public int UtcOffset { get; set; } = 0;
        public string Language { get; set; }
        public DateTime? JWTExpiration { get; set; } 
    }
} 