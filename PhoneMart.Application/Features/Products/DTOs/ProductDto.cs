namespace PhoneMart.Application.Features.Products.DTOs;

/// <summary>
/// OUTPUT DTO — What the frontend receives when requesting a product.
/// This controls exactly what data leaves the backend.
/// We never send the raw Product entity because:
///   1. Entity may have navigation properties that cause circular references
///   2. We might want to include computed fields (like CategoryName)
///   3. We control what the frontend sees (security)
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }

    // Shop info (so frontend knows which shop this belongs to)
    public Guid ShopId { get; set; }
    public string ShopName { get; set; } = "";

    // Category & Brand — we send the NAME not just the ID (better for frontend)
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";

    public int? BrandId { get; set; }
    public string? BrandName { get; set; }

    // Product details
    public string Title { get; set; } = "";
    public decimal Price { get; set; }

    // Phone-specific fields (null for accessories)
    public string? Condition { get; set; }
    public string? Storage { get; set; }
    public string? Warranty { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public string Status { get; set; } = "";   // "Active" / "Sold" / "Hidden" (string, not enum number)
    public DateTime CreatedAt { get; set; }
}
