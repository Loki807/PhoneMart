using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Wholesale.Requests.Commands;
using PhoneMart.Application.Features.Wholesale.Validators;
using PhoneMart.Domain.Entities;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Wholesale.Handlers.Commands;

/// <summary>
/// HANDLER: Updates an existing wholesale listing.
/// 
/// Security: Verifies the listing belongs to the owner's shop.
/// 
/// Common use case:
///   Owner sold 20 out of 50 screens → updates AvailableQty to 30
///   Owner sold all → changes Status to Sold
/// </summary>
public class UpdateWholesaleHandler : IRequestHandler<UpdateWholesaleCommand, Guid>
{
    private readonly IAppDbContext _db;

    public UpdateWholesaleHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(UpdateWholesaleCommand request, CancellationToken cancellationToken)
    {
        UpdateWholesaleValidator.Validate(request);

        var dto = request.WholesaleDto;

        // Find owner's shop
        var shop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.OwnerUserId);
        if (shop == null)
            throw new Exception("You don't have a shop.");

        // Find the listing
        var listing = await _db.FindAsync<WholesaleListing>(dto.ListingId, cancellationToken);
        if (listing == null)
            throw new Exception("Wholesale listing not found.");

        // SECURITY: verify ownership
        if (listing.SellerShopId != shop.Id)
            throw new Exception("You can only update your own listings.");

        // Update fields
        listing.ItemType = (WholesaleItemType)dto.ItemType;
        listing.Title = dto.Title.Trim();
        listing.UnitPrice = dto.UnitPrice;
        listing.MinOrderQty = dto.MinOrderQty;
        listing.AvailableQty = dto.AvailableQty;
        listing.Condition = dto.Condition?.Trim();
        listing.Description = dto.Description?.Trim();
        listing.ImageUrl = dto.ImageUrl?.Trim();
        listing.Status = (ListingStatus)dto.Status;

        _db.UpdateEntity(listing);
        await _db.SaveChangesAsync(cancellationToken);

        return listing.Id;
    }
}
