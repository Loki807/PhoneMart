using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Products.Requests.Commands;
using PhoneMart.Application.Features.Products.Validators;
using PhoneMart.Domain.Entities;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Products.Handlers.Commands;

/// <summary>
/// HANDLER: Processes CreateProductCommand
/// 
/// Flow:
///   1. Validate input
///   2. Find the owner's shop (using OwnerUserId from JWT)
///   3. Create the Product entity
///   4. Save to database
///   5. Return the new product ID
/// 
/// SECURITY: The ShopId comes from the database (owner's shop),
/// NOT from the request body. This prevents a malicious user
/// from adding products to someone else's shop.
/// </summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateProductHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate input data
        CreateProductValidator.Validate(request);

        var dto = request.ProductDto;

        // Step 2: Find the owner's shop
        // We look up the Shop where OwnerUserId matches the logged-in user
        var shop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.OwnerUserId);

        if (shop == null)
            throw new Exception("You don't have a shop. Contact admin.");

        // Step 3: Create the Product entity
        var product = new Product
        {
            Id = Guid.NewGuid(),
            ShopId = shop.Id,               // ← Comes from DB, not from request!
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            Title = dto.Title.Trim(),
            Price = dto.Price,
            Condition = dto.Condition?.Trim(),
            Storage = dto.Storage?.Trim(),
            Warranty = dto.Warranty?.Trim(),
            Description = dto.Description?.Trim(),
            ImageUrl = dto.ImageUrl?.Trim(),
            Status = ListingStatus.Active,   // New products are always Active
            CreatedAt = DateTime.UtcNow
        };

        // Step 4: Save to database
        await _db.AddEntityAsync(product, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        // Step 5: Return the new product ID
        return product.Id;
    }
}
