namespace coer91
{
    public class ResponseList<T> : ResponseEnumerableBuilder<T>
    {
        public override IEnumerable<T> Data { get; set; }
    }
} 