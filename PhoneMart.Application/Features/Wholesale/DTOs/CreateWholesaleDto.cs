namespace PhoneMart.Application.Features.Wholesale.DTOs;

/// <summary>
/// INPUT DTO — What the frontend sends when CREATING a wholesale listing.
/// 
/// Business Context:
///   A shop owner posts: "I have 50 iPhone screens, Rs.5000 each, min order 10"
///   Other shop owners see this and contact via WhatsApp to buy.
/// 
/// Same security pattern as Products:
///   - No SellerShopId here (comes from JWT → owner's shop)
///   - Prevents posting listings under someone else's shop
/// </summary>
public class CreateWholesaleDto
{
    // What type? (1=Phone, 2=Accessory, 3=SparePart, 4=RepairItem)
    public int ItemType { get; set; }

    public string Title { get; set; } = "";
    public decimal UnitPrice { get; set; }         // Price per single unit

    public int MinOrderQty { get; set; } = 1;      // Minimum pieces buyer must take
    public int AvailableQty { get; set; } = 1;     // Total pieces available

    public string? Condition { get; set; }          // "New" / "Used"
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }           // AWS S3 URL
}
