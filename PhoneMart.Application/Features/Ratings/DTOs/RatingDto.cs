namespace PhoneMart.Application.Features.Ratings.DTOs;

/// <summary>
/// OUTPUT DTO — What the frontend receives for a rating.
/// Shows who rated, how many stars, and a comment.
/// </summary>
public class RatingDto
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }

    // Who gave the rating
    public string RaterShopName { get; set; } = "";    // "Jaffna Mobile Hub"
    public string RaterCity { get; set; } = "";         // "Jaffna"

    public int Stars { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
