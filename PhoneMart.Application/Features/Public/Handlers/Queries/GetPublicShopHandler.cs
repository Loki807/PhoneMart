using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Public.DTOs;
using PhoneMart.Application.Features.Public.Requests.Queries;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Public.Handlers.Queries;

/// <summary>
/// HANDLER: Returns a shop's public page data.
/// 
/// Only returns verified shops (IsVerified = true).
/// Includes ProductCount so customers know how many items are listed.
/// </summary>
public class GetPublicShopHandler : IRequestHandler<GetPublicShopQuery, PublicShopDto?>
{
    private readonly IAppDbContext _db;

    public GetPublicShopHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<PublicShopDto?> Handle(GetPublicShopQuery request, CancellationToken cancellationToken)
    {
        var data = _db.Shops
            .Where(s => s.Id == request.ShopId)
            .Select(s => new PublicShopDto
            {
                Id = s.Id,
                ShopName = s.ShopName,
                Address = s.Address,
                City = s.City,
                WhatsAppNumber = s.WhatsAppNumber,
                CallNumber = s.CallNumber,
                ImageUrl = s.ImageUrl,
                ProductCount = _db.Products
                    .Count(p => p.ShopId == s.Id && p.Status == ListingStatus.Active),
                TotalRatings = _db.Ratings.Count(r => r.ShopId == s.Id),
                AverageStars = _db.Ratings.Where(r => r.ShopId == s.Id).Any()
                    ? Math.Round(_db.Ratings.Where(r => r.ShopId == s.Id).Average(r => (double)r.Stars), 1)
                    : 0,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefault();

        return Task.FromResult(data);
    }
}
