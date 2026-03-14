using MediatR;
using PhoneMart.Application.Features.Public.DTOs;

namespace PhoneMart.Application.Features.Public.Requests.Queries;

/// <summary>
/// QUERY: "Show me a shop's public page"
/// No auth needed — anyone can view a shop.
/// </summary>
public class GetPublicShopQuery : IRequest<PublicShopDto?>
{
    public Guid ShopId { get; set; }
}
