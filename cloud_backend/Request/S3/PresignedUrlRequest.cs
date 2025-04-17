namespace cloud_backend.Request.S3
{
    public class PresignedUrlRequest
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
