using MediatR;

namespace PhoneMart.Application.Features.Shops.Requests.Commands;

public class DeleteShopCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
