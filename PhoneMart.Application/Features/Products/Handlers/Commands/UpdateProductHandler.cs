using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Products.Requests.Commands;
using PhoneMart.Application.Features.Products.Validators;
using PhoneMart.Domain.Entities;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Products.Handlers.Commands;

/// <summary>
/// HANDLER: Processes UpdateProductCommand
/// 
/// SECURITY CHECK: We verify the product belongs to the owner's shop.
/// Even if an attacker sends someone else's product ID,
/// we reject it because we check: product.ShopId == owner's shop ID.
/// </summary>
public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Guid>
{
    private readonly IAppDbContext _db;

    public UpdateProductHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate input
        UpdateProductValidator.Validate(request);

        var dto = request.ProductDto;

        // Step 2: Find the owner's shop
        var shop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.OwnerUserId);
        if (shop == null)
            throw new Exception("You don't have a shop.");

        // Step 3: Find the product
        var product = await _db.FindAsync<Product>(dto.ProductId, cancellationToken);
        if (product == null)
            throw new Exception("Product not found.");

        // Step 4: SECURITY — verify this product belongs to MY shop
        if (product.ShopId != shop.Id)
            throw new Exception("You can only update your own products.");

        // Step 5: Update all fields
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.Title = dto.Title.Trim();
        product.Price = dto.Price;
        product.Condition = dto.Condition?.Trim();
        product.Storage = dto.Storage?.Trim();
        product.Warranty = dto.Warranty?.Trim();
        product.Description = dto.Description?.Trim();
        product.ImageUrl = dto.ImageUrl?.Trim();
        product.Status = (ListingStatus)dto.Status;

        // Step 6: Save changes
        _db.UpdateEntity(product);
        await _db.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
