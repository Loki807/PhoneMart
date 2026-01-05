using MediatR;
using PhoneMart.Application.Features.Shops.DTOs;

namespace PhoneMart.Application.Features.Shops.Requests.Commands;

public class UpdateShopCommand : IRequest<Guid>
{
    public UpdateShopDto ShopDto { get; set; } = default!;
}
