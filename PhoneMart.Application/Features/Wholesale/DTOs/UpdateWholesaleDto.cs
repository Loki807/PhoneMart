namespace PhoneMart.Application.Features.Wholesale.DTOs;

/// <summary>
/// INPUT DTO — What the frontend sends when UPDATING a wholesale listing.
/// 
/// Use cases:
///   - Update quantity after selling some items
///   - Change price
///   - Mark as Sold (status=2) when all items are sold
///   - Hide listing temporarily (status=3)
/// </summary>
public class UpdateWholesaleDto
{
    public Guid ListingId { get; set; }            // Which listing to update

    public int ItemType { get; set; }
    public string Title { get; set; } = "";
    public decimal UnitPrice { get; set; }

    public int MinOrderQty { get; set; } = 1;
    public int AvailableQty { get; set; } = 1;

    public string? Condition { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int Status { get; set; } = 1;           // 1=Active, 2=Sold, 3=Hidden
}
