namespace coer91
{
    public class ResponseBulkUpload<T> : ResponseDTOBuilder<BulkUploadDTO<T>>
    {
        public override BulkUploadDTO<T> Data { get; set; }
    }
}