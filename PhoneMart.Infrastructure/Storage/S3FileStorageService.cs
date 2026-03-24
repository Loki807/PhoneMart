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
/// File naming:
///   We generate unique names using GUID to prevent conflicts.
///   Example: products/a1b2c3d4-photo.jpg
/// 
/// Configuration needed in appsettings.json:
///   "AWS": {
///     "BucketName": "phonemart-images",
///     "Region": "ap-south-1",
///     "AccessKey": "your-key",
///     "SecretKey": "your-secret"
///   }
/// </summary>
public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3FileStorageService(IConfiguration config)
    {
        var awsConfig = config.GetSection("AWS");
        _bucketName = awsConfig["BucketName"] ?? "phonemart-images";

        var accessKey = awsConfig["AccessKey"] ?? "";
        var secretKey = awsConfig["SecretKey"] ?? "";
        var region = awsConfig["Region"] ?? "ap-south-1";

        if (!string.IsNullOrEmpty(accessKey) && !accessKey.Contains("YOUR_AWS"))
        {
            _s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }
        else
        {
            // Fallback to Default Credentials (e.g. IAM Instance Profile on EC2/EB)
            _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region));
        }
    }

    /// <summary>
    /// Upload file to S3 and return its public URL.
    /// 
    /// Flow:
    ///   1. Generate unique filename (GUID + original extension)
    ///   2. Set the S3 key (folder/filename)
    ///   3. Upload with public-read ACL (so the image is viewable)
    ///   4. Return the public URL
    /// </summary>
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder)
    {
        // Generate unique name: "products/a1b2c3d4-photo.jpg"
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

        await _s3Client.PutObjectAsync(request);

        // Return the public URL
        return $"https://{_bucketName}.s3.amazonaws.com/{uniqueName}";
    }

    /// <summary>
    /// Delete file from S3 by its URL.
    /// Extracts the key from the URL and deletes the object.
    /// </summary>
    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        // Extract key from URL: "https://bucket.s3.amazonaws.com/products/file.jpg" → "products/file.jpg"
        var uri = new Uri(fileUrl);
        var key = uri.AbsolutePath.TrimStart('/');

        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(request);
    }

    /// <summary>
    /// Maps file extensions to MIME types for proper browser rendering.
    /// Without this, browsers might download instead of display the image.
    /// </summary>
    private static string GetContentType(string extension) => extension.ToLower() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };
}
