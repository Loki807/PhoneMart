using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Shops.Requests.Commands;
using PhoneMart.Domain.Entities;

namespace PhoneMart.Application.Features.Shops.Handlers.Commands;

public class DeleteShopHandler : IRequestHandler<DeleteShopCommand, bool>
{
    private readonly IAppDbContext _db;

    public DeleteShopHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeleteShopCommand request, CancellationToken cancellationToken)
    {
        var shop = await _db.FindAsync<Shop>(request.Id, cancellationToken);
        if (shop == null) return false;

        // ✅ Disable owner (safe)
        var owner = await _db.FindAsync<User>(shop.OwnerUserId, cancellationToken);
        if (owner != null)
        {
            owner.IsActive = false;
            _db.UpdateEntity(owner);
        }

        // ✅ Delete shop
        _db.RemoveEntity(shop);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
