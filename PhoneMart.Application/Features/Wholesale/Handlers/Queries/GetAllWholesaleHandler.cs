using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Wholesale.DTOs;
using PhoneMart.Application.Features.Wholesale.Requests.Queries;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Wholesale.Handlers.Queries;

/// <summary>
/// HANDLER: THE WHOLESALE MARKETPLACE
/// 
/// This is the heart of the wholesale feature.
/// Returns ALL active wholesale listings from ALL shops,
/// with seller contact info so buyers can reach out.
/// 
/// Business Flow:
///   1. Any logged-in shop owner opens the marketplace
///   2. Sees listings from all shops (newest first)
///   3. Can filter by ItemType (Phone/Accessory/SparePart/RepairItem)
///   4. Can filter by seller's city
///   5. Clicks WhatsApp/Call to contact the seller
///   6. They negotiate and trade offline
/// 
/// IMPORTANT: Only shows Active listings (not Sold or Hidden)
/// because buyers shouldn't see sold-out items.
/// </summary>
public class GetAllWholesaleHandler : IRequestHandler<GetAllWholesaleQuery, List<WholesaleListingDto>>
{
    private readonly IAppDbContext _db;

    public GetAllWholesaleHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<List<WholesaleListingDto>> Handle(GetAllWholesaleQuery request, CancellationToken cancellationToken)
    {
        // Start with all active listings
        var query = _db.WholesaleListings
            .Where(w => w.Status == ListingStatus.Active);

        // Apply optional ItemType filter
        if (request.ItemTypeFilter.HasValue)
        {
            var itemType = (WholesaleItemType)request.ItemTypeFilter.Value;
            query = query.Where(w => w.ItemType == itemType);
        }

        // Build the result with seller info from Shops table
        var listings = query
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WholesaleListingDto
            {
                Id = w.Id,
                SellerShopId = w.SellerShopId,

                // JOIN with Shops to get seller details
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
            .ToList();

        // Apply city filter in memory (after getting seller info)
        if (!string.IsNullOrWhiteSpace(request.CityFilter))
        {
            listings = listings
                .Where(l => l.SellerCity.Contains(request.CityFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Task.FromResult(listings);
    }
}
