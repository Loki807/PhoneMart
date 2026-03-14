using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Products.DTOs;
using PhoneMart.Application.Features.Products.Requests.Queries;

namespace PhoneMart.Application.Features.Products.Handlers.Queries;

/// <summary>
/// HANDLER: Processes GetProductByIdQuery
/// 
/// Returns a single product with all details.
/// This is used by both owners (to edit) and potentially public views later.
/// </summary>
public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IAppDbContext _db;

    public GetProductByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var data = _db.Products
            .Where(p => p.Id == request.ProductId)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ShopId = p.ShopId,
                ShopName = _db.Shops
                    .Where(s => s.Id == p.ShopId)
                    .Select(s => s.ShopName)
                    .FirstOrDefault() ?? "",

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
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefault();

        return Task.FromResult(data);
    }
}
