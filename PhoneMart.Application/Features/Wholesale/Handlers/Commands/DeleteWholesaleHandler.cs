using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Wholesale.Requests.Commands;
using PhoneMart.Domain.Entities;

namespace PhoneMart.Application.Features.Wholesale.Handlers.Commands;

/// <summary>
/// HANDLER: Deletes a wholesale listing.
/// Hard delete — the listing is fully removed from the database.
/// </summary>
public class DeleteWholesaleHandler : IRequestHandler<DeleteWholesaleCommand, bool>
{
    private readonly IAppDbContext _db;

    public DeleteWholesaleHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeleteWholesaleCommand request, CancellationToken cancellationToken)
    {
        var shop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.OwnerUserId);
        if (shop == null) return false;

        var listing = await _db.FindAsync<WholesaleListing>(request.ListingId, cancellationToken);
        if (listing == null) return false;

        // SECURITY: verify ownership
        if (listing.SellerShopId != shop.Id)
            throw new Exception("You can only delete your own listings.");

        _db.RemoveEntity(listing);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
