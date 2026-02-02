namespace coer91
{
    public class ResponseDTO<T> : ResponseDTOBuilder<T>
    {
        public override T Data { get; set; }
    }
} 