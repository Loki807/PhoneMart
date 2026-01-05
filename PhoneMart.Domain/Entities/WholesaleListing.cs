using PhoneMart.Domain.Enums;

namespace PhoneMart.Domain.Entities;

public class WholesaleListing
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Relationship: Many wholesale listings belong to One Shop (seller)
    public Guid SellerShopId { get; set; }
    public Shop SellerShop { get; set; } = default!;

    public WholesaleItemType ItemType { get; set; } = WholesaleItemType.Phone;

    public string Title { get; set; } = "";
    public decimal UnitPrice { get; set; }

    public int MinOrderQty { get; set; } = 1;
    public int AvailableQty { get; set; } = 1;

    public string? Condition { get; set; } // New/Used
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public ListingStatus Status { get; set; } = ListingStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
