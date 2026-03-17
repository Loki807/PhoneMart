using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneMart.Application.Contracts.Storage;

namespace PhoneMart.API.Controllers;

/// <summary>
/// IMAGE UPLOAD CONTROLLER

/// 
/// Handles file uploads for product and wholesale listing images.
/// Only logged-in owners can upload images.
/// 
/// Flow:
///   1. Frontend selects a file (image)
///   2. Frontend sends it as multipart/form-data to this endpoint
///   3. Backend uploads to AWS S3
///   4. Backend returns the public URL
///   5. Frontend uses this URL when creating/updating a product
/// 
/// Why separate from product creation?
///   Because file upload and JSON data are different content types.
///   It's cleaner to upload the image first, get the URL, then
///   include that URL in the create/update product JSON body.
/// 
/// ROUTE: /api/images/...
/// </summary>
[ApiController]
[Route("api/images")]
[Authorize(Roles = "Owner,Admin")]
public class ImageController : ControllerBase
{
    private readonly IFileStorageService _storage;

    public ImageController(IFileStorageService storage)
    {
        _storage = storage;
    }

    /// <summary>
    /// POST /api/images/upload?folder=products
    /// POST /api/images/upload?folder=wholesale
    /// 
    /// Upload an image file. Returns the public URL.
    /// 
    /// Usage from frontend (JavaScript example):
    ///   const formData = new FormData();
    ///   formData.append('file', selectedFile);
    ///   fetch('/api/images/upload?folder=products', {
    ///     method: 'POST',
    ///     headers: { 'Authorization': 'Bearer ' + token },
    ///     body: formData
    ///   });
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromQuery] string folder = "products")
    {
        // Validate: file must exist
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        // Validate: max 5MB
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "File too large. Max 5MB." });

        // Validate: must be an image
        var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedTypes.Contains(ext))
            return BadRequest(new { message = "Only image files allowed (jpg, png, gif, webp)." });

        // Validate: folder must be valid
        var allowedFolders = new[] { "products", "wholesale", "shops" };
        if (!allowedFolders.Contains(folder.ToLower()))
            return BadRequest(new { message = "Folder must be 'products', 'wholesale', or 'shops'." });

        // Upload to S3
        using var stream = file.OpenReadStream();
        var url = await _storage.UploadFileAsync(stream, file.FileName, folder.ToLower());

        return Ok(new { imageUrl = url });
    }

    /// <summary>
    /// DELETE /api/images?url=https://bucket.s3.amazonaws.com/products/file.jpg
    /// 
    /// Delete an image from S3.
    /// Used when a product/wholesale listing is deleted or image is replaced.
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest(new { message = "URL is required." });

        await _storage.DeleteFileAsync(url);

        return Ok(new { deleted = true });
    }
}
