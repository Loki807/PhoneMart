namespace PhoneMart.Application.Features.Public.DTOs;

/// <summary>
/// OUTPUT DTO — Shop info for public visitors (no login needed).
/// 
/// Difference from ShopDetailsDto (admin version):
///   - INCLUDES WhatsApp/Call (so customers can contact)
///   - INCLUDES product count (so customers know how many items this shop has)
///   - EXCLUDES OwnerUserId (internal, not public)
///   - EXCLUDES IsVerified (internal admin flag)
///   
/// Business: A customer browsing PhoneMart sees this info for each shop.
/// </summary>
public class PublicShopDto
{
    public Guid Id { get; set; }

    public string ShopName { get; set; } = "";
    public string? Address { get; set; }
    public string City { get; set; } = "";

    // Contact buttons — the main way customers reach the shop
    public string WhatsAppNumber { get; set; } = "";
    public string CallNumber { get; set; } = "";
    public string? ImageUrl { get; set; }

    // How many products this shop currently has listed
    public int ProductCount { get; set; }

    // Rating summary — shows shop reputation
    public double AverageStars { get; set; }     // e.g., 4.3
    public int TotalRatings { get; set; }        // e.g., 12

    public DateTime CreatedAt { get; set; }
}
