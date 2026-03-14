namespace PhoneMart.Application.Features.Public.DTOs;

/// <summary>
/// OUTPUT DTO — Product info for public visitors.
/// 
/// Same as ProductDto but also includes shop contact info,
/// so a customer can directly contact the shop about this product.
/// 
/// Business: Customer sees iPhone 15 → clicks WhatsApp → talks to shop owner.
/// </summary>
public class PublicProductDto
{
    public Guid Id { get; set; }

    // Shop info (so customer knows which shop sells this)
    public Guid ShopId { get; set; }
    public string ShopName { get; set; } = "";
    public string ShopCity { get; set; } = "";
    public string ShopWhatsApp { get; set; } = "";
    public string ShopCallNumber { get; set; } = "";

    // Category & Brand
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }

    // Product details
    public string Title { get; set; } = "";
    public decimal Price { get; set; }

    public string? Condition { get; set; }
    public string? Storage { get; set; }
    public string? Warranty { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
