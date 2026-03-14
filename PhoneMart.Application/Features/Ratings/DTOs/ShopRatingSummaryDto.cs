namespace PhoneMart.Application.Features.Ratings.DTOs;

/// <summary>
/// OUTPUT DTO — Shop rating summary (average + count).
/// Added to the public shop page response.
/// </summary>
public class ShopRatingSummaryDto
{
    public double AverageStars { get; set; }   // e.g., 4.3
    public int TotalRatings { get; set; }      // e.g., 12
}
