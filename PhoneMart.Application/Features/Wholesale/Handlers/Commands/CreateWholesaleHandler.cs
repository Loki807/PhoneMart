using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Wholesale.Requests.Commands;
using PhoneMart.Application.Features.Wholesale.Validators;
using PhoneMart.Domain.Entities;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Wholesale.Handlers.Commands;

/// <summary>
/// HANDLER: Creates a new wholesale listing.
/// 
/// Same security pattern as CreateProductHandler:
///   - SellerShopId comes from the owner's shop (via JWT), NOT from the request
///   - Validates input before processing
///   - New listings always start as Active
/// </summary>
public class CreateWholesaleHandler : IRequestHandler<CreateWholesaleCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateWholesaleHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateWholesaleCommand request, CancellationToken cancellationToken)
    {
        CreateWholesaleValidator.Validate(request);

        var dto = request.WholesaleDto;

        // Find the owner's shop (secure: from JWT, not from request)
        var shop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.OwnerUserId);
        if (shop == null)
            throw new Exception("You don't have a shop. Contact admin.");

        var listing = new WholesaleListing
        {
            Id = Guid.NewGuid(),
            SellerShopId = shop.Id,                          // From DB, not request!
            ItemType = (WholesaleItemType)dto.ItemType,
            Title = dto.Title.Trim(),
            UnitPrice = dto.UnitPrice,
            MinOrderQty = dto.MinOrderQty,
            AvailableQty = dto.AvailableQty,
            Condition = dto.Condition?.Trim(),
            Description = dto.Description?.Trim(),
            ImageUrl = dto.ImageUrl?.Trim(),
            Status = ListingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _db.AddEntityAsync(listing, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return listing.Id;
    }
}
