using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneMart.Application.Features.Products.DTOs;
using PhoneMart.Application.Features.Products.Requests.Commands;
using PhoneMart.Application.Features.Products.Requests.Queries;
using PhoneMart.Application.Features.Shops.DTOs;
using PhoneMart.Application.Features.Shops.Requests.Queries;
using PhoneMart.Application.Features.Wholesale.DTOs;
using PhoneMart.Application.Features.Wholesale.Requests.Commands;
using PhoneMart.Application.Features.Wholesale.Requests.Queries;
using PhoneMart.Application.Features.Ratings.DTOs;
using PhoneMart.Application.Features.Ratings.Requests.Commands;


namespace PhoneMart.API.Controllers;

/// <summary>
/// SHOP OWNER CONTROLLER
/// 
/// This controller handles all actions a Shop Owner can do:
///   - View their own shop details
///   - Add / Edit / Delete products
/// 
/// SECURITY:
///   - [Authorize(Roles = "Owner")] → Only users with Role=Owner can access
///   - GetUserId() extracts the user ID from the JWT token
///   - Every action uses this userId to find the owner's shop
///   - An owner can NEVER access another owner's data
/// 
/// ROUTE: All endpoints start with /api/shop/...
/// 
/// HOW JWT USER ID EXTRACTION WORKS:
///   When the user logs in, we put their ID in the JWT token as "sub" (subject).
///   When they make a request, the JWT middleware reads the token and puts
///   the claims into HttpContext.User. We then read the "sub" claim to get the user ID.
/// </summary>
[ApiController]
[Route("api/shop")]
[Authorize(Roles = "Owner")]   // ← ONLY shop owners can access these endpoints
public class ShopOwnerController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShopOwnerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ─────────────────────────────────────────────
    //  HELPER: Extract User ID from JWT token
    // ─────────────────────────────────────────────

    /// <summary>
    /// Reads the "sub" (subject) claim from the JWT token.
    /// This is the logged-in user's ID.
    /// 
    /// The claim was set in JwtTokenGenerator.cs:
    ///   new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
    /// </summary>
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null)
            throw new Exception("User ID not found in token.");

        return Guid.Parse(userIdClaim.Value);
    }

    // ═════════════════════════════════════════════
    //  MY SHOP — View & Update my shop details
    // ═════════════════════════════════════════════

    /// <summary>
    /// GET /api/shop/my-shop
    /// 
    /// Returns the logged-in owner's shop details.
    /// Uses the same GetShopByIdQuery that Admin uses,
    /// but we first find the shop by OwnerUserId.
    /// </summary>
    [HttpGet("my-shop")]
    public async Task<IActionResult> GetMyShop(CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        // We need to get the shop ID for this owner
        // Using the existing query infrastructure
        var result = await _mediator.Send(new GetAllShopsQuery(), cancellationToken);
        var myShop = result.FirstOrDefault(s => s.OwnerUserId == userId);

        if (myShop == null)
            return NotFound(new { message = "You don't have a shop yet." });

        return Ok(myShop);
    }

    // ═════════════════════════════════════════════
    //  PRODUCTS — Full CRUD for my shop's products
    // ═════════════════════════════════════════════

    /// <summary>
    /// GET /api/shop/products
    /// 
    /// Returns all products belonging to the logged-in owner's shop.
    /// Newest products appear first.
    /// </summary>
    [HttpGet("products")]
    public async Task<IActionResult> GetMyProducts(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var products = await _mediator.Send(
            new GetMyProductsQuery { OwnerUserId = userId },
            cancellationToken);

        return Ok(products);
    }

    /// <summary>
    /// GET /api/shop/products/{id}
    /// 
    /// Returns a single product by ID.
    /// Note: Any owner can view any product (for now).
    /// In the future, we might restrict this.
    /// </summary>
    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetProductByIdQuery { ProductId = id },
            cancellationToken);

        if (result == null)
            return NotFound(new { message = "Product not found." });

        return Ok(result);
    }

    /// <summary>
    /// POST /api/shop/products
    /// 
    /// Creates a new product in the owner's shop.
    /// 
    /// Example request body:
    /// {
    ///     "categoryId": 1,
    ///     "brandId": null,
    ///     "title": "iPhone 15 Pro Max 256GB",
    ///     "price": 450000,
    ///     "condition": "Like New",
    ///     "storage": "256GB",
    ///     "warranty": "1 Month Shop Warranty",
    ///     "description": "Used for 2 months, no scratches",
    ///     "imageUrl": null
    /// }
    /// </summary>
    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new CreateProductCommand
        {
            OwnerUserId = userId,          // ← Secure: from JWT, not from body
            ProductDto = dto
        };

        var productId = await _mediator.Send(command, cancellationToken);

        return Ok(new { productId });
    }

    /// <summary>
    /// PUT /api/shop/products
    /// 
    /// Updates an existing product.
    /// The handler verifies the product belongs to this owner.
    /// </summary>
    [HttpPut("products")]
    public async Task<IActionResult> UpdateProduct(
        [FromBody] UpdateProductDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new UpdateProductCommand
        {
            OwnerUserId = userId,
            ProductDto = dto
        };

        var productId = await _mediator.Send(command, cancellationToken);

        return Ok(new { productId });
    }

    /// <summary>
    /// DELETE /api/shop/products/{id}
    /// 
    /// Deletes a product from the owner's shop.
    /// The handler verifies ownership before deleting.
    /// </summary>
    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new DeleteProductCommand
        {
            OwnerUserId = userId,
            ProductId = id
        };

        var ok = await _mediator.Send(command, cancellationToken);

        if (!ok)
            return NotFound(new { message = "Product not found." });

        return Ok(new { deleted = true });
    }

    // ═════════════════════════════════════════════
    //  WHOLESALE — CRUD for my shop's wholesale listings
    // ═════════════════════════════════════════════

    /// <summary>
    /// GET /api/shop/wholesale
    /// 
    /// Returns all wholesale listings belonging to the logged-in owner's shop.
    /// </summary>
    [HttpGet("wholesale")]
    public async Task<IActionResult> GetMyWholesale(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var listings = await _mediator.Send(
            new GetMyWholesaleQuery { OwnerUserId = userId },
            cancellationToken);

        return Ok(listings);
    }

    /// <summary>
    /// POST /api/shop/wholesale
    /// 
    /// Creates a new wholesale listing in the owner's shop.
    /// 
    /// Example request body:
    /// {
    ///     "itemType": 3,
    ///     "title": "50x iPhone 14 Screens (Original)",
    ///     "unitPrice": 5000,
    ///     "minOrderQty": 10,
    ///     "availableQty": 50,
    ///     "condition": "New",
    ///     "description": "Original iPhone 14 screens, bulk order",
    ///     "imageUrl": null
    /// }
    /// </summary>
    [HttpPost("wholesale")]
    public async Task<IActionResult> CreateWholesale(
        [FromBody] CreateWholesaleDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new CreateWholesaleCommand
        {
            OwnerUserId = userId,
            WholesaleDto = dto
        };

        var listingId = await _mediator.Send(command, cancellationToken);

        return Ok(new { listingId });
    }

    /// <summary>
    /// PUT /api/shop/wholesale
    /// 
    /// Updates an existing wholesale listing.
    /// The handler verifies the listing belongs to this owner's shop.
    /// </summary>
    [HttpPut("wholesale")]
    public async Task<IActionResult> UpdateWholesale(
        [FromBody] UpdateWholesaleDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new UpdateWholesaleCommand
        {
            OwnerUserId = userId,
            WholesaleDto = dto
        };

        var listingId = await _mediator.Send(command, cancellationToken);

        return Ok(new { listingId });
    }

    /// <summary>
    /// DELETE /api/shop/wholesale/{id}
    /// 
    /// Deletes a wholesale listing from the owner's shop.
    /// The handler verifies ownership before deleting.
    /// </summary>
    [HttpDelete("wholesale/{id:guid}")]
    public async Task<IActionResult> DeleteWholesale(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new DeleteWholesaleCommand
        {
            OwnerUserId = userId,
            ListingId = id
        };

        var ok = await _mediator.Send(command, cancellationToken);

        if (!ok)
            return NotFound(new { message = "Wholesale listing not found." });

        return Ok(new { deleted = true });
    }

    // ═════════════════════════════════════════════
    //  RATINGS — Rate other shops
    // ═════════════════════════════════════════════

    /// <summary>
    /// POST /api/shop/ratings
    /// 
    /// Rate another shop (after a wholesale interaction).
    /// If you already rated this shop, your rating will be UPDATED.
    /// You CANNOT rate your own shop.
    /// 
    /// Example request body:
    /// {
    ///     "shopId": "guid-of-shop-to-rate",
    ///     "stars": 5,
    ///     "comment": "Great quality screens, fast delivery!"
    /// }
    /// </summary>
    [HttpPost("ratings")]
    public async Task<IActionResult> RateShop(
        [FromBody] CreateRatingDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new CreateRatingCommand
        {
            RaterUserId = userId,
            RatingDto = dto
        };

        var ratingId = await _mediator.Send(command, cancellationToken);

        return Ok(new { ratingId });
    }
}
