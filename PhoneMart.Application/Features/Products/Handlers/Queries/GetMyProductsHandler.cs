using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Products.DTOs;
using PhoneMart.Application.Features.Products.Requests.Queries;

namespace PhoneMart.Application.Features.Products.Handlers.Queries;

/// <summary>
/// HANDLER: Processes GetMyProductsQuery
/// 
/// Returns all products belonging to the logged-in owner's shop.
/// 
/// Notice the LINQ query:
///   - We JOIN with Shops, Categories, and Brands to get names
///   - We project into ProductDto (not returning the raw entity)
///   - OrderByDescending(CreatedAt) = newest products first
/// </summary>
public class GetMyProductsHandler : IRequestHandler<GetMyProductsQuery, List<ProductDto>>
{
    private readonly IAppDbContext _db;

    public GetMyProductsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<List<ProductDto>> Handle(GetMyProductsQuery request, CancellationToken cancellationToken)
    {
        // Step 1: Find the owner's shop
        var shop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.OwnerUserId);

        if (shop == null)
            return Task.FromResult(new List<ProductDto>());  // No shop = empty list

        // Step 2: Get all products for this shop, with category & brand names
        var products = _db.Products
            .Where(p => p.ShopId == shop.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ShopId = p.ShopId,
                ShopName = shop.ShopName,

                CategoryId = p.CategoryId,
                CategoryName = _db.Categories
                    .Where(c => c.Id == p.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefault() ?? "",

                BrandId = p.BrandId,
                BrandName = p.BrandId != null
                    ? _db.Brands
                        .Where(b => b.Id == p.BrandId)
                        .Select(b => b.Name)
                        .FirstOrDefault()
                    : null,

                Title = p.Title,
                Price = p.Price,
                Condition = p.Condition,
                Storage = p.Storage,
                Warranty = p.Warranty,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Status = p.Status.ToString(),    // Enum → "Active" / "Sold" / "Hidden"
                CreatedAt = p.CreatedAt
            })
            .ToList();

        return Task.FromResult(products);
    }
}
