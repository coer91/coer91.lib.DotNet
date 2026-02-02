namespace coer91
{
    public abstract class Response  
    {
        public bool Failure { get; set; } = false;
        public int HttpCode { get; set; } = 200;
        public List<string> MessageList { get; set; } = [];
    }


    public abstract class Response<T> : Response
    {
        public virtual T Data { get; set; }
    }


    public abstract class ResponseEnumerable<T> : Response
    {
        public virtual IEnumerable<T> Data { get; set; }
    }
}