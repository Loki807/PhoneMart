using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneMart.Application.Features.Wholesale.Requests.Queries;

namespace PhoneMart.API.Controllers;

/// <summary>
/// WHOLESALE MARKETPLACE CONTROLLER
/// 
/// This is the COMMON WHOLESALE PAGE where all logged-in shop owners
/// can browse wholesale listings from ALL shops.
/// 
/// Business flow:
///   1. Owner opens the wholesale marketplace
///   2. Sees all active listings (newest first)
///   3. Can filter by ItemType (Phone/Accessory/SparePart/RepairItem)
///   4. Can filter by seller's city
///   5. Clicks on a listing → sees seller's WhatsApp/Call number
///   6. Contacts the seller to negotiate and trade
/// 
/// SECURITY: Only logged-in Owners can access (not public, not admin).
/// This protects seller contact info from being scraped.
/// 
/// ROUTE: /api/wholesale/...
/// </summary>
[ApiController]
[Route("api/wholesale")]
[Authorize(Roles = "Owner")]
public class WholesaleController : ControllerBase
{
    private readonly IMediator _mediator;

    public WholesaleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/wholesale
    /// 
    /// Browse all active wholesale listings from all shops.
    /// 
    /// Optional query parameters:
    ///   ?itemType=1  → Filter by type (1=Phone, 2=Accessory, 3=SparePart, 4=RepairItem)
    ///   ?city=Jaffna → Filter by seller's city
    /// 
    /// Example: GET /api/wholesale?itemType=3&city=Jaffna
    ///   → Shows all spare parts from Jaffna shops
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllWholesale(
        [FromQuery] int? itemType,
        [FromQuery] string? city,
        CancellationToken cancellationToken)
    {
        var query = new GetAllWholesaleQuery
        {
            ItemTypeFilter = itemType,
            CityFilter = city
        };

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// GET /api/wholesale/{id}
    /// 
    /// View a single wholesale listing with full seller contact info.
    /// The frontend can use the sellerWhatsApp and sellerCallNumber
    /// to show WhatsApp and Call buttons.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWholesaleById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetWholesaleByIdQuery { ListingId = id },
            cancellationToken);

        if (result == null)
            return NotFound(new { message = "Wholesale listing not found." });

        return Ok(result);
    }
}
