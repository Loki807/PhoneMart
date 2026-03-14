using PhoneMart.Application.Features.Products.Requests.Commands;

namespace PhoneMart.Application.Features.Products.Validators;

/// <summary>
/// VALIDATOR: Checks UpdateProductCommand input before processing.
/// Same rules as create, plus the ProductId must be valid.
/// </summary>
public static class UpdateProductValidator
{
    public static void Validate(UpdateProductCommand request)
    {
        if (request.ProductDto == null)
            throw new Exception("ProductDto is required.");

        var dto = request.ProductDto;

        if (dto.ProductId == Guid.Empty)
            throw new Exception("ProductId is required.");

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new Exception("Title is required.");

        if (dto.Title.Length > 200)
            throw new Exception("Title must be 200 characters or less.");

        if (dto.Price <= 0)
            throw new Exception("Price must be greater than 0.");

        if (dto.CategoryId < 1 || dto.CategoryId > 3)
            throw new Exception("CategoryId must be 1 (Used Phone), 2 (New Phone), or 3 (Accessory).");

        if (dto.CategoryId is 1 or 2 && string.IsNullOrWhiteSpace(dto.Condition))
            throw new Exception("Condition is required for phones.");

        // Status must be a valid ListingStatus value
        if (dto.Status < 1 || dto.Status > 3)
            throw new Exception("Status must be 1 (Active), 2 (Sold), or 3 (Hidden).");
    }
}
