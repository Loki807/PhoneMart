using MediatR;
using PhoneMart.Application.Features.Wholesale.DTOs;

namespace PhoneMart.Application.Features.Wholesale.Requests.Queries;

/// <summary>
/// QUERY: "Show me ALL wholesale listings from ALL shops"
/// 
/// This is the WHOLESALE MARKETPLACE — where any logged-in shop owner
/// can browse what other shops are selling in bulk.
/// 
/// Only shows Active listings (not Sold or Hidden).
/// 
/// Business Flow:
///   1. Owner browses marketplace → sees listings from all shops
///   2. Finds something interesting (e.g., "50x iPhone screens, Rs.5000 each")
///   3. Clicks WhatsApp/Call button → contacts the seller
///   4. They negotiate and trade offline
/// </summary>
public class GetAllWholesaleQuery : IRequest<List<WholesaleListingDto>>
{
    // Optional filters (for future search/filter feature)
    public int? ItemTypeFilter { get; set; }       // Filter by type (Phone/Accessory/etc.)
    public string? CityFilter { get; set; }        // Filter by seller's city
}
