using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Shops.DTOs;
using PhoneMart.Application.Features.Shops.Requests.Queries;

namespace PhoneMart.Application.Features.Shops.Handlers.Queries;

public class GetShopByIdHandler : IRequestHandler<GetShopByIdQuery, ShopDetailsDto?>
{
    private readonly IAppDbContext _db;

    public GetShopByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<ShopDetailsDto?> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
    {
        var data = _db.Shops
            .Where(s => s.Id == request.Id)
            .Select(s => new ShopDetailsDto
            {
                Id = s.Id,
                OwnerUserId = s.OwnerUserId,
                OwnerEmail = _db.Users.Where(u => u.Id == s.OwnerUserId).Select(u => u.Email).FirstOrDefault() ?? "",

                ShopName = s.ShopName,
                Address = s.Address,
                City = s.City,
                WhatsAppNumber = s.WhatsAppNumber,
                CallNumber = s.CallNumber,
                IsVerified = s.IsVerified,
                ImageUrl = s.ImageUrl,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefault();

        return Task.FromResult(data);
    }
}
