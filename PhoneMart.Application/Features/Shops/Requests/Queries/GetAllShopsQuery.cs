using MediatR;
using PhoneMart.Application.Features.Shops.DTOs;

namespace PhoneMart.Application.Features.Shops.Requests.Queries;

public class GetAllShopsQuery : IRequest<List<ShopDetailsDto>>
{
}
