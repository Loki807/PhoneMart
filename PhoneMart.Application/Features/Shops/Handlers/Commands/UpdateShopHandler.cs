using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Shops.Requests.Commands;
using PhoneMart.Application.Features.Shops.Validators;
using PhoneMart.Domain.Entities;

namespace PhoneMart.Application.Features.Shops.Handlers.Commands;

public class UpdateShopHandler : IRequestHandler<UpdateShopCommand, Guid>
{
    private readonly IAppDbContext _db;

    public UpdateShopHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(UpdateShopCommand request, CancellationToken cancellationToken)
    {
        UpdateShopValidator.Validate(request);

        var dto = request.ShopDto;

        var shop = await _db.FindAsync<Shop>(dto.ShopId, cancellationToken);
        if (shop == null)
            throw new Exception("Shop not found.");

        shop.ShopName = dto.ShopName.Trim();
        shop.Address = dto.Address?.Trim();
        shop.City = dto.City.Trim();
        shop.WhatsAppNumber = dto.WhatsAppNumber?.Trim() ?? "";
        shop.CallNumber = dto.CallNumber.Trim();
        shop.IsVerified = dto.IsVerified;
        shop.ImageUrl = dto.ImageUrl?.Trim();

        _db.UpdateEntity(shop);
        await _db.SaveChangesAsync(cancellationToken);

        return shop.Id;
    }
}
