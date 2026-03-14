namespace PhoneMart.Application.Features.Wholesale.DTOs;

/// <summary>
/// OUTPUT DTO — What the frontend receives for a wholesale listing.
/// 
/// Key difference from ProductDto:
///   - Has SellerShopName, SellerWhatsApp, SellerCallNumber
///     (so buyers can contact the seller directly)
///   - Has MinOrderQty and AvailableQty (bulk-specific fields)
///   - Has ItemType as string ("Phone" / "Accessory" / "SparePart" / "RepairItem")
/// </summary>
public class WholesaleListingDto
{
    public Guid Id { get; set; }

    // Seller info — so buyer can see WHO is selling and CONTACT them
    public Guid SellerShopId { get; set; }
    public string SellerShopName { get; set; } = "";
    public string SellerCity { get; set; } = "";
    public string SellerWhatsApp { get; set; } = "";    // For WhatsApp button
    public string SellerCallNumber { get; set; } = "";  // For Call button

    // Listing details
    public string ItemType { get; set; } = "";          // "Phone" / "Accessory" / etc.
    public string Title { get; set; } = "";
    public decimal UnitPrice { get; set; }

    public int MinOrderQty { get; set; }
    public int AvailableQty { get; set; }

    public string? Condition { get; set; }              // "New" / "Used"
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public string Status { get; set; } = "";            // "Active" / "Sold" / "Hidden"
    public DateTime CreatedAt { get; set; }
}
