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
                _settings.AccessKey,
                _settings.SecretKey,
                _settings.SessionToken
            );

            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_settings.Region)
            };

            _s3Client = new AmazonS3Client(credentials, config);

        }

        public string GeneratePresignedUrl(string extension)
        {
            var fileName = $"{Guid.NewGuid()}.{extension}";
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = fileName,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddHours(8).AddMinutes(10),
                ContentType = $"image/{extension}"
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}
