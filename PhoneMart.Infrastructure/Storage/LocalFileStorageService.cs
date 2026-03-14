using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PhoneMart.Application.Contracts.Storage;

namespace PhoneMart.Infrastructure.Storage;

/// <summary>
/// LOCAL FILE STORAGE — Saves images to wwwroot/uploads/ folder.
/// 
/// This is a local alternative to S3FileStorageService.
/// Files are stored on disk and served via ASP.NET Core static files.
/// 
/// The returned URL format is: /uploads/{folder}/{guid}{ext}
/// e.g., /uploads/products/a1b2c3d4.jpg
/// 
/// The frontend prepends the backend base URL (http://localhost:5236) 
/// to get the full image URL.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsRoot;
    private readonly string _baseUrl;

    public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config)
    {
        // Store files in wwwroot/uploads/
        _uploadsRoot = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");

        // Base URL for building the full image URL
        // Default: http://localhost:5236
        _baseUrl = config["App:BaseUrl"] ?? "http://localhost:5236";
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder)
    {
        // Ensure folder exists: wwwroot/uploads/products/ or wwwroot/uploads/wholesale/
        var folderPath = Path.Combine(_uploadsRoot, folder);
        Directory.CreateDirectory(folderPath);

        // Generate unique filename
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, uniqueName);

        // Save file to disk
        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs);
        }

        // Return full URL so frontend can display the image
        return $"{_baseUrl}/uploads/{folder}/{uniqueName}";
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return Task.CompletedTask;

        try
        {
            // Extract path from URL: http://localhost:5236/uploads/products/file.jpg → uploads/products/file.jpg
            var uri = new Uri(fileUrl);
            var relativePath = uri.AbsolutePath.TrimStart('/'); // "uploads/products/file.jpg"
            var fullPath = Path.Combine(Path.GetDirectoryName(_uploadsRoot)!, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch
        {
            // Silently ignore delete errors
        }

        return Task.CompletedTask;
    }
}
