namespace coer91.NET
{
    public class ResponseDTO<T> : ResponseDTOBuilder<T>
    {
        public override T Data { get; set; }
    }
} 