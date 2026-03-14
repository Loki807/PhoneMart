using MediatR;
using PhoneMart.Application.Features.Ratings.DTOs;

namespace PhoneMart.Application.Features.Ratings.Requests.Queries;

/// <summary>
/// QUERY: "Get all ratings for a shop"
/// Used on the public shop page to show reviews.
/// </summary>
public class GetShopRatingsQuery : IRequest<List<RatingDto>>
{
    public Guid ShopId { get; set; }
}
