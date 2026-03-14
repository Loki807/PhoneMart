using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Ratings.DTOs;
using PhoneMart.Application.Features.Ratings.Requests.Queries;

namespace PhoneMart.Application.Features.Ratings.Handlers.Queries;

/// <summary>
/// HANDLER: Returns all ratings for a specific shop.
/// 
/// Each rating shows WHO rated (their shop name + city)
/// so the reader knows it's from a real shop owner.
/// 
/// Newest ratings first.
/// </summary>
public class GetShopRatingsHandler : IRequestHandler<GetShopRatingsQuery, List<RatingDto>>
{
    private readonly IAppDbContext _db;

    public GetShopRatingsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<List<RatingDto>> Handle(GetShopRatingsQuery request, CancellationToken cancellationToken)
    {
        var ratings = _db.Ratings
            .Where(r => r.ShopId == request.ShopId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RatingDto
            {
                Id = r.Id,
                ShopId = r.ShopId,

                // Get the rater's shop info
                RaterShopName = _db.Shops
                    .Where(s => s.OwnerUserId == r.RaterUserId)
                    .Select(s => s.ShopName)
                    .FirstOrDefault() ?? "Unknown",

                RaterCity = _db.Shops
                    .Where(s => s.OwnerUserId == r.RaterUserId)
                    .Select(s => s.City)
                    .FirstOrDefault() ?? "",

                Stars = r.Stars,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToList();

        return Task.FromResult(ratings);
    }
}
