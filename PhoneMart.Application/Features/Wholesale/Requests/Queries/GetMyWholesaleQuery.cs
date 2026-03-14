using MediatR;
using PhoneMart.Application.Features.Wholesale.DTOs;

namespace PhoneMart.Application.Features.Wholesale.Requests.Queries;

/// <summary>
/// QUERY: "Give me MY wholesale listings"
/// 
/// Used by ShopOwnerController to show the owner their own listings.
/// </summary>
public class GetMyWholesaleQuery : IRequest<List<WholesaleListingDto>>
{
    public Guid OwnerUserId { get; set; }
}
