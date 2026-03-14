using PhoneMart.Application.Features.Products.Requests.Commands;

namespace PhoneMart.Application.Features.Products.Validators;

/// <summary>
/// VALIDATOR: Checks CreateProductCommand input before processing.
/// 
/// Why validate here (not in controller)?
/// Because the HANDLER is where business logic lives.
/// The controller is just a "waiter" — it shouldn't know business rules.
/// 
/// Example: "Title must not be empty" is a business rule, not an API rule.
/// </summary>
public static class CreateProductValidator
{
    public static void Validate(CreateProductCommand request)
    {
        if (request.ProductDto == null)
            throw new Exception("ProductDto is required.");

        var dto = request.ProductDto;

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new Exception("Title is required.");

        if (dto.Title.Length > 200)
            throw new Exception("Title must be 200 characters or less.");

        if (dto.Price <= 0)
            throw new Exception("Price must be greater than 0.");

        if (dto.CategoryId < 1 || dto.CategoryId > 3)
            throw new Exception("CategoryId must be 1 (Used Phone), 2 (New Phone), or 3 (Accessory).");

        // Condition is required for phones (Category 1 or 2), not for accessories (3)
        if (dto.CategoryId is 1 or 2 && string.IsNullOrWhiteSpace(dto.Condition))
            throw new Exception("Condition is required for phones (Like New / Good / Average).");
    }
}
