using PhoneMart.Domain.Enums;

namespace PhoneMart.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Relationship: Many Products belong to One Shop
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = default!;

    // Relationship: Many Products belong to One Category (Used/New/Accessories)
    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    // Optional brand (some accessories may not have brand)
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public string Title { get; set; } = "";
    public decimal Price { get; set; }

    // Used-phone fields
    public string? Condition { get; set; } // Like New / Good / Average
    public string? Storage { get; set; }   // 64GB/128GB/256GB
    public string? Warranty { get; set; }  // "1 Month Shop Warranty"

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int StockQuantity { get; set; } = 1;  // How many units in stock

    public ListingStatus Status { get; set; } = ListingStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
