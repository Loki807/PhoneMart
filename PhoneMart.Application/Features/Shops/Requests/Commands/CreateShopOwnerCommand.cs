using MediatR;
using PhoneMart.Application.Features.Shops.DTOs;

namespace PhoneMart.Application.Features.Shops.Requests.Commands;

public class CreateShopOwnerCommand : IRequest<Guid>
{
    public CreateShopOwnerDto ShopOwnerDto { get; set; } = default!;
}
