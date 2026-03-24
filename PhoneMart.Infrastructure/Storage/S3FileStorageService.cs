using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using PhoneMart.Application.Contracts.Storage;

namespace PhoneMart.Infrastructure.Storage;

/// <summary>
/// AWS S3 FILE STORAGE — Uploads images to Amazon S3.
/// 
/// How AWS S3 works:
///   1. You create a "bucket" (like a folder in the cloud)
///   2. You upload files to the bucket
///   3. Each file gets a public URL
///   4. Frontend uses this URL to display the image
/// 
/// Configuration needed in appsettings.json:
///   "AWS": {
///     "BucketName": "phonemart-images",
///     "Region": "ap-southeast-2",
///     "AccessKey": "your-key",
///     "SecretKey": "your-secret"
///   }
/// </summary>
public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _region;
    private readonly string? _cdnBaseUrl;

    public S3FileStorageService(IConfiguration config)
    {
        var awsConfig = config.GetSection("AWS");
        _bucketName = awsConfig["BucketName"] ?? "phonemart-images";
        _region = awsConfig["Region"] ?? "ap-southeast-2";
        
        // App:BaseUrl normally points to CloudFront or the main domain
        _cdnBaseUrl = config["App:BaseUrl"];

        var accessKey = awsConfig["AccessKey"];
        var secretKey = awsConfig["SecretKey"];

        if (!string.IsNullOrEmpty(accessKey) && !accessKey.Contains("YOUR_AWS"))
        {
            _s3Client = new AmazonS3Client(
                accessKey,
                secretKey,
                Amazon.RegionEndpoint.GetBySystemName(_region)
            );
        }
        else
        {
            // Fallback to Default Credentials (e.g. IAM Instance Profile on EC2/EB)
            _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(_region));
        }
    }

    /// <summary>
    /// Upload file to S3 and return its public URL.
    /// </summary>
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{folder}/{Guid.NewGuid()}{extension}";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = uniqueName,
            InputStream = fileStream,
            ContentType = GetContentType(extension),
            CannedACL = S3CannedACL.PublicRead
        };

        try
        {
            await _s3Client.PutObjectAsync(request);
            
            // If CloudFront/CDN URL is configured, use it for the returned link.
            // Example: https://d1krxot6naclbu.cloudfront.net/products/abc-123.jpg
            if (!string.IsNullOrEmpty(_cdnBaseUrl))
            {
                var cleanBase = _cdnBaseUrl.TrimEnd('/');
                return $"{cleanBase}/{uniqueName}";
            }

            // Direct S3 Regional URL: https://bucket.s3.region.amazonaws.com/uniqueName
            return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{uniqueName}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"S3 UPLOAD FAILED: {ex.Message}");
            throw new Exception("Image upload to AWS failed. Check your Bucket Policy and ACLs on the AWS S3 Console.", ex);
        }
    }

    /// <summary>
    /// Delete file from S3 by its URL.
    /// </summary>
    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        try
        {
            var uri = new Uri(fileUrl);
            var key = uri.AbsolutePath.TrimStart('/');

            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }
        catch
        {
            // Ignore errors on delete to prevent breaking product deletion
        }
    }

    private static string GetContentType(string extension) => extension.ToLower() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };
}
