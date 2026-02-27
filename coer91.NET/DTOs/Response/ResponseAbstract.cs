namespace coer91.NET
{
    public abstract class ResponseAbstract  
    {
        public bool Failure { get; set; } = false;
        public int HttpCode { get; set; } = 200;
        public List<string> MessageList { get; set; } = [];
    }


    public abstract class ResponseAbstract<T> : ResponseAbstract
    {
        public virtual T Data { get; set; }
    }


    public abstract class ResponseListAbstrac<T> : ResponseAbstract
    {
        public virtual List<T> Data { get; set; }
    }
}