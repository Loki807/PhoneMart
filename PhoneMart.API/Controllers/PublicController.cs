using MediatR;
using Microsoft.AspNetCore.Mvc;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Public.Requests.Queries;
using PhoneMart.Application.Features.Ratings.Requests.Queries;

namespace PhoneMart.API.Controllers;

/// <summary>
/// PUBLIC CONTROLLER — No Authentication Required!
/// 
/// This controller serves the PUBLIC-facing part of PhoneMart.
/// Anyone (customer) can browse shops, view products, and search
/// WITHOUT logging in.
/// 
/// Business Flow:
///   1. Customer visits PhoneMart website
///   2. Searches for "iPhone 15" or browses a shop
///   3. Sees products with shop contact info (WhatsApp/Call)
///   4. Contacts the shop directly to buy
/// 
/// ROUTE: /api/public/...
/// NO [Authorize] attribute = anyone can access
/// </summary>
[ApiController]
[Route("api/public")]
public class PublicController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAppDbContext _db;

    public PublicController(IMediator mediator, IAppDbContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    // ═════════════════════════════════════════════
    //  SHOPS
    // ═════════════════════════════════════════════

    /// <summary>
    /// GET /api/public/shops/{id}
    /// 
    /// View a single shop's public page.
    /// Shows shop name, address, city, contact numbers, and product count.
    /// </summary>
    [HttpGet("shops/{id:guid}")]
    public async Task<IActionResult> GetShop(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPublicShopQuery { ShopId = id },
            cancellationToken);

        if (result == null)
            return NotFound(new { message = "Shop not found." });

        return Ok(result);
    }

    /// <summary>
    /// GET /api/public/shops/{id}/products
    /// GET /api/public/shops/{id}/products?category=1
    /// 
    /// View all active products in a specific shop.
    /// Optional: filter by category (1=Used, 2=New, 3=Accessories).
    /// </summary>
    [HttpGet("shops/{id:guid}/products")]
    public async Task<IActionResult> GetShopProducts(
        Guid id,
        [FromQuery] int? category,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPublicShopProductsQuery
            {
                ShopId = id,
                CategoryFilter = category
            },
            cancellationToken);

        return Ok(result);
    }

    // ═════════════════════════════════════════════
    //  SEARCH
    // ═════════════════════════════════════════════

    /// <summary>
    /// GET /api/public/search?q=iPhone
    /// GET /api/public/search?q=Samsung&category=2&city=Jaffna
    /// 
    /// Search products across ALL shops.
    /// Returns matching active products with shop contact info.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string? q,
        [FromQuery] int? category,
        [FromQuery] string? city,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SearchProductsQuery
            {
                SearchTerm = q,
                CategoryFilter = category,
                CityFilter = city
            },
            cancellationToken);

        return Ok(result);
    }

    // ═════════════════════════════════════════════
    //  LOOKUPS (Categories & Brands)
    // ═════════════════════════════════════════════

    /// <summary>
    /// GET /api/public/categories
    /// 
    /// Returns all categories. Used by frontend for dropdown/filter.
    /// Currently: Used Phones(1), New Phones(2), Accessories(3)
    /// </summary>
    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var categories = _db.Categories
            .OrderBy(c => c.Id)
            .Select(c => new { c.Id, c.Name })
            .ToList();

        return Ok(categories);
    }

    /// <summary>
    /// GET /api/public/brands
    /// 
    /// Returns all brands. Used by frontend for dropdown/filter.
    /// </summary>
    [HttpGet("brands")]
    public IActionResult GetBrands()
    {
        var brands = _db.Brands
            .OrderBy(b => b.Name)
            .Select(b => new { b.Id, b.Name })
            .ToList();

        return Ok(brands);
    }

    // ═════════════════════════════════════════════
    //  RATINGS
    // ═════════════════════════════════════════════

    /// <summary>
    /// GET /api/public/shops/{id}/ratings
    /// 
    /// View all ratings for a shop. Shows who rated and their comment.
    /// No login required — anyone can see reviews.
    /// </summary>
    [HttpGet("shops/{id:guid}/ratings")]
    public async Task<IActionResult> GetShopRatings(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetShopRatingsQuery { ShopId = id },
            cancellationToken);

        return Ok(result);
    }
}
