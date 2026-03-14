using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Wholesale.DTOs;
using PhoneMart.Application.Features.Wholesale.Requests.Queries;

namespace PhoneMart.Application.Features.Wholesale.Handlers.Queries;

/// <summary>
/// HANDLER: Returns the owner's own wholesale listings.
/// </summary>
public class GetMyWholesaleHandler : IRequestHandler<GetMyWholesaleQuery, List<WholesaleListingDto>>
{
    private readonly IAppDbContext _db;

    public GetMyWholesaleHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<List<WholesaleListingDto>> Handle(GetMyWholesaleQuery request, CancellationToken cancellationToken)
    {
        var shop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.OwnerUserId);

        if (shop == null)
            return Task.FromResult(new List<WholesaleListingDto>());

        var listings = _db.WholesaleListings
            .Where(w => w.SellerShopId == shop.Id)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WholesaleListingDto
            {
                Id = w.Id,
                SellerShopId = w.SellerShopId,
                SellerShopName = shop.ShopName,
                SellerCity = shop.City,
                SellerWhatsApp = shop.WhatsAppNumber,
                SellerCallNumber = shop.CallNumber,

                ItemType = w.ItemType.ToString(),
                Title = w.Title,
                UnitPrice = w.UnitPrice,
                MinOrderQty = w.MinOrderQty,
                AvailableQty = w.AvailableQty,
                Condition = w.Condition,
                Description = w.Description,
                ImageUrl = w.ImageUrl,
                Status = w.Status.ToString(),
                CreatedAt = w.CreatedAt
            })
            .ToList();

        return Task.FromResult(listings);
    }
}
