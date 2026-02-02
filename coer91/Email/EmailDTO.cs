namespace coer91
{
    public class EmailDTO
    {
        public IEnumerable<string> To { get; set; }
        public IEnumerable<string> CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}