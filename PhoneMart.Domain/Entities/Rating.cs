namespace PhoneMart.Domain.Entities;

/// <summary>
/// RATING ENTITY — A shop owner rates another shop.
/// 
/// Business Context:
///   After a wholesale transaction, the buyer rates the seller's shop.
///   Example: "Jaffna Mobile Hub" buys screens from "Colombo Parts" 
///   → rates them 5 stars: "Great quality, fast delivery!"
/// 
/// Rules:
///   - Stars: 1 to 5 (whole numbers only)
///   - One rating per rater per shop (can't spam ratings)
///   - Can't rate your own shop
///   - Comment is optional but encouraged
/// </summary>
public class Rating
{
    public Guid Id { get; set; }

    // Which shop is being rated
    public Guid ShopId { get; set; }

    // Who gave the rating (a shop owner's userId)
    public Guid RaterUserId { get; set; }

    // Rating details
    public int Stars { get; set; }            // 1 to 5
    public string? Comment { get; set; }       // Optional review text

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Shop Shop { get; set; } = null!;
    public User Rater { get; set; } = null!;
}
