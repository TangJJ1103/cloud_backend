namespace cloud_backend.Request.S3
{
    public class S3Settings
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string SessionToken { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
    }
}
