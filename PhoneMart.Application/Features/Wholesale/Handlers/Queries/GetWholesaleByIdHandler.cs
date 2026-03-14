using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Wholesale.DTOs;
using PhoneMart.Application.Features.Wholesale.Requests.Queries;

namespace PhoneMart.Application.Features.Wholesale.Handlers.Queries;

/// <summary>
/// HANDLER: Returns a single wholesale listing with full seller contact info.
/// </summary>
public class GetWholesaleByIdHandler : IRequestHandler<GetWholesaleByIdQuery, WholesaleListingDto?>
{
    private readonly IAppDbContext _db;

    public GetWholesaleByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<WholesaleListingDto?> Handle(GetWholesaleByIdQuery request, CancellationToken cancellationToken)
    {
        var data = _db.WholesaleListings
            .Where(w => w.Id == request.ListingId)
            .Select(w => new WholesaleListingDto
            {
                Id = w.Id,
                SellerShopId = w.SellerShopId,
                SellerShopName = _db.Shops
                    .Where(s => s.Id == w.SellerShopId)
                    .Select(s => s.ShopName)
                    .FirstOrDefault() ?? "",
                SellerCity = _db.Shops
                    .Where(s => s.Id == w.SellerShopId)
                    .Select(s => s.City)
                    .FirstOrDefault() ?? "",
                SellerWhatsApp = _db.Shops
                    .Where(s => s.Id == w.SellerShopId)
                    .Select(s => s.WhatsAppNumber)
                    .FirstOrDefault() ?? "",
                SellerCallNumber = _db.Shops
                    .Where(s => s.Id == w.SellerShopId)
                    .Select(s => s.CallNumber)
                    .FirstOrDefault() ?? "",

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
            .FirstOrDefault();

        return Task.FromResult(data);
    }
}
