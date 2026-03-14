using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Public.DTOs;
using PhoneMart.Application.Features.Public.Requests.Queries;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Public.Handlers.Queries;

/// <summary>
/// HANDLER: Searches products across ALL shops.
/// 
/// Business: Customer types "iPhone" → sees matching products from every shop.
/// 
/// Search matches against product Title (case-insensitive).
/// Only Active products from verified shops are shown.
/// 
/// Supports filters:
///   - CategoryFilter: 1=Used, 2=New, 3=Accessories
///   - CityFilter: filter by shop's city
/// </summary>
public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, List<PublicProductDto>>
{
    private readonly IAppDbContext _db;

    public SearchProductsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<List<PublicProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        // Start with all active products
        var query = _db.Products
            .Where(p => p.Status == ListingStatus.Active);

        // Apply search term (matches product Title)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(p => p.Title.ToLower().Contains(term));
        }

        // Apply category filter
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

        // Apply city filter in memory (city comes from shop sub-query)
        if (!string.IsNullOrWhiteSpace(request.CityFilter))
        {
            products = products
                .Where(p => p.ShopCity.Contains(request.CityFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Task.FromResult(products);
    }
}
