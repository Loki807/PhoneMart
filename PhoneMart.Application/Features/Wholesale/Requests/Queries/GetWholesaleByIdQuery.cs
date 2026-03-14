using MediatR;
using PhoneMart.Application.Features.Wholesale.DTOs;

namespace PhoneMart.Application.Features.Wholesale.Requests.Queries;

/// <summary>
/// QUERY: "Get details of a single wholesale listing"
/// 
/// Returns full details including seller contact info.
/// </summary>
public class GetWholesaleByIdQuery : IRequest<WholesaleListingDto?>
{
    public Guid ListingId { get; set; }
}
