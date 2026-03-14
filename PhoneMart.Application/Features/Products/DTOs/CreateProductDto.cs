namespace PhoneMart.Application.Features.Products.DTOs;

/// <summary>
/// INPUT DTO — What the frontend sends when CREATING a new product.
/// 
/// Notice: No Id field! The backend generates the Id.
/// Notice: No ShopId! The backend gets it from the logged-in user's JWT token.
/// This prevents a shop owner from adding products to someone else's shop.
/// </summary>
public class CreateProductDto
{
    // Which type? (1=Used Phone, 2=New Phone, 3=Accessory)
    public int CategoryId { get; set; }

    // Optional brand (Samsung=?, Apple=?, etc.)
    public int? BrandId { get; set; }

    // Product info
    public string Title { get; set; } = "";
    public decimal Price { get; set; }

    // Phone-specific (optional — leave null for accessories)
    public string? Condition { get; set; }   // "Like New" / "Good" / "Average"
    public string? Storage { get; set; }     // "64GB" / "128GB" / "256GB"
    public string? Warranty { get; set; }    // "1 Month Shop Warranty"

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }    // AWS S3 URL (uploaded separately)
}
