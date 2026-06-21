using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Infrastructure.Services
{
    public class AwsS3Service : IImageService
    {
        private readonly string _bucketName;
        private readonly AmazonS3Client _s3Client;

        public AwsS3Service(IConfiguration configuration)
        {
            var awsConfig = configuration.GetSection("AWS");
            var accessKey = awsConfig["AccessKey"];
            var secretKey = awsConfig["SecretKey"];
            _bucketName = awsConfig["BucketName"]!;
            var region = RegionEndpoint.GetBySystemName(awsConfig["Region"]);

            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            _s3Client = new AmazonS3Client(credentials, region);
        }

        public async Task<List<string>> UploadImagesAsync(IEnumerable<FileUploadDto> files, CancellationToken cancellationToken)
        {
            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                // Generate uniqe file name
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"products/{uniqueFileName}",
                    InputStream = file.Content,
                    ContentType = file.ContentType
                };

                await _s3Client.PutObjectAsync(putRequest, cancellationToken);

                // Public URL
                var url = $"https://{_bucketName}.s3.{_s3Client.Config.RegionEndpoint.SystemName}.amazonaws.com/products/{uniqueFileName}";
                imageUrls.Add(url);
            }

            return imageUrls;
        }
    }
}
