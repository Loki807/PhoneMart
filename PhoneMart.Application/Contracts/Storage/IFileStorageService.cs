namespace PhoneMart.Application.Contracts.Storage;

/// <summary>
/// CONTRACT: File storage service interface.
/// 
/// Why an interface? Same reason as IAuthService:
///   - Application layer defines WHAT it needs
///   - Infrastructure layer provides HOW (AWS S3)
///   - If you switch from S3 to Azure Blob later, only the implementation changes
/// 
/// Returns the public URL of the uploaded file.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Upload a file and return its public URL.
    /// </summary>
    /// <param name="fileStream">The file data</param>
    /// <param name="fileName">Original filename (e.g., "photo.jpg")</param>
    /// <param name="folder">Folder in S3 (e.g., "products" or "wholesale")</param>
    /// <returns>Public URL of the uploaded file</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);

    /// <summary>
    /// Delete a file from storage.
    /// </summary>
    /// <param name="fileUrl">The full URL of the file to delete</param>
    Task DeleteFileAsync(string fileUrl);
}
