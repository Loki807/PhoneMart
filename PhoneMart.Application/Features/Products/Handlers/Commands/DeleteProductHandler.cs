using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Products.Requests.Commands;
using PhoneMart.Domain.Entities;

namespace PhoneMart.Application.Features.Products.Handlers.Commands;

/// <summary>
/// HANDLER: Processes DeleteProductCommand
/// 
/// Business Rule: Products are HARD deleted (actually removed from DB).
/// Unlike Users which are soft-deleted (IsActive=false),
/// products are fully removed because there's no need to keep
/// deleted product records.
/// 
/// SECURITY: Same ownership check as Update.
/// </summary>
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IAppDbContext _db;

    public DeleteProductHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Find the owner's shop
        var shop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.OwnerUserId);
        if (shop == null)
            return false;

        // Step 2: Find the product
        var product = await _db.FindAsync<Product>(request.ProductId, cancellationToken);
        if (product == null)
            return false;

        // Step 3: SECURITY — verify this product belongs to MY shop
        if (product.ShopId != shop.Id)
            throw new Exception("You can only delete your own products.");

        // Step 4: Delete from database
        _db.RemoveEntity(product);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
