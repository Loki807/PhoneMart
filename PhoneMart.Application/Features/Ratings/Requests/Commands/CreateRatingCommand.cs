using MediatR;
using PhoneMart.Application.Features.Ratings.DTOs;

namespace PhoneMart.Application.Features.Ratings.Requests.Commands;

/// <summary>
/// COMMAND: "I want to rate a shop"
/// 
/// If the owner already rated this shop, it UPDATES the existing rating.
/// This is called an "upsert" pattern (update or insert).
/// </summary>
public class CreateRatingCommand : IRequest<Guid>
{
    public Guid RaterUserId { get; set; }              // From JWT
    public CreateRatingDto RatingDto { get; set; } = default!;
}
