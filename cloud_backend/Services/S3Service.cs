using Amazon.S3.Model;
using Amazon.S3;
using cloud_backend.Request.S3;
using Microsoft.Extensions.Options;
using Amazon.Runtime;

namespace cloud_backend.Services
{
    public class S3Service
    {
        private readonly S3Settings _settings;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IOptions<S3Settings> settings)
        {
            _settings = settings.Value;

            var credentials = new SessionAWSCredentials(
                Environment.GetEnvironmentVariable("ACCESS_KEY"),
                Environment.GetEnvironmentVariable("SECRET_KEY"),
                Environment.GetEnvironmentVariable("SESSION_TOKEN")
            );

            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_settings.Region)
            };

            _s3Client = new AmazonS3Client(credentials, config);

        }

        public async Task<string> UploadImageToS3Async(Stream imageStream, string contentType, string fileName)
        {
            var key = $"images/{fileName}";

            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = imageStream,
                ContentType = contentType,
                AutoCloseStream = true
            };

            await _s3Client.PutObjectAsync(request);

            return $"https://{_settings.BucketName}.s3.amazonaws.com/{key}";
        }
    }
}
