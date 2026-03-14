namespace PhoneMart.Application.Features.Ratings.DTOs;

/// <summary>
/// INPUT DTO — What the frontend sends when rating a shop.
/// 
/// No RaterUserId — comes from JWT (secure).
/// </summary>
public class CreateRatingDto
{
    public Guid ShopId { get; set; }       // Which shop to rate
    public int Stars { get; set; }          // 1 to 5
    public string? Comment { get; set; }    // Optional review text
}
