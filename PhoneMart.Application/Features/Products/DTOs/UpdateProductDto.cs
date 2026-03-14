namespace PhoneMart.Application.Features.Products.DTOs;

/// <summary>
/// INPUT DTO — What the frontend sends when UPDATING an existing product.
/// 
/// Notice: ProductId is required (which product to update).
/// Notice: No ShopId — the handler verifies the product belongs to the logged-in owner.
/// This prevents an owner from updating someone else's product.
/// </summary>
public class UpdateProductDto
{
    public Guid ProductId { get; set; }      // Which product to update

    public int CategoryId { get; set; }
    public int? BrandId { get; set; }

    public string Title { get; set; } = "";
    public decimal Price { get; set; }

    public string? Condition { get; set; }
    public string? Storage { get; set; }
    public string? Warranty { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // Owner can change status (Active → Hidden, Hidden → Active, etc.)
    public int Status { get; set; } = 1;     // 1=Active, 2=Sold, 3=Hidden
}
