using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Public.DTOs;
using PhoneMart.Application.Features.Public.Requests.Queries;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Public.Handlers.Queries;

/// <summary>
/// HANDLER: Returns all ACTIVE products for a specific shop.
/// 
/// Only Active products shown (customers shouldn't see Sold/Hidden).
/// Includes shop contact info so customer can call about any product.
/// </summary>
public class GetPublicShopProductsHandler : IRequestHandler<GetPublicShopProductsQuery, List<PublicProductDto>>
{
    private readonly IAppDbContext _db;

    public GetPublicShopProductsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<List<PublicProductDto>> Handle(GetPublicShopProductsQuery request, CancellationToken cancellationToken)
    {
        // Start with active products for this shop
        var query = _db.Products
            .Where(p => p.ShopId == request.ShopId && p.Status == ListingStatus.Active);

        // Apply optional category filter
        if (request.CategoryFilter.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryFilter.Value);

        var products = query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PublicProductDto
            {
                Id = p.Id,
                ShopId = p.ShopId,
                ShopName = _db.Shops.Where(s => s.Id == p.ShopId).Select(s => s.ShopName).FirstOrDefault() ?? "",
                ShopCity = _db.Shops.Where(s => s.Id == p.ShopId).Select(s => s.City).FirstOrDefault() ?? "",
                ShopWhatsApp = _db.Shops.Where(s => s.Id == p.ShopId).Select(s => s.WhatsAppNumber).FirstOrDefault() ?? "",
                ShopCallNumber = _db.Shops.Where(s => s.Id == p.ShopId).Select(s => s.CallNumber).FirstOrDefault() ?? "",

                CategoryId = p.CategoryId,
                CategoryName = _db.Categories.Where(c => c.Id == p.CategoryId).Select(c => c.Name).FirstOrDefault() ?? "",
                BrandId = p.BrandId,
                BrandName = p.BrandId != null
                    ? _db.Brands.Where(b => b.Id == p.BrandId).Select(b => b.Name).FirstOrDefault()
                    : null,

                Title = p.Title,
                Price = p.Price,
                Condition = p.Condition,
                Storage = p.Storage,
                Warranty = p.Warranty,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt
            })
            .ToList();

        return Task.FromResult(products);
    }
}
