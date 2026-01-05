using MediatR;
using PhoneMart.Application.Features.Shops.DTOs;

namespace PhoneMart.Application.Features.Shops.Requests.Queries;

public class GetShopByIdQuery : IRequest<ShopDetailsDto?>
{
    public Guid Id { get; set; }
}
