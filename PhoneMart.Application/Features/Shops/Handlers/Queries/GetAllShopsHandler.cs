using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Shops.DTOs;
using PhoneMart.Application.Features.Shops.Requests.Queries;

namespace PhoneMart.Application.Features.Shops.Handlers.Queries;

public class GetAllShopsHandler : IRequestHandler<GetAllShopsQuery, List<ShopDetailsDto>>
{
    private readonly IAppDbContext _db;

    public GetAllShopsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public Task<List<ShopDetailsDto>> Handle(GetAllShopsQuery request, CancellationToken cancellationToken)
    {
        var list = _db.Shops
            .OrderByDescending(s => s.CreatedAt)
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
                CreatedAt = s.CreatedAt
            })
            .ToList();

        return Task.FromResult(list);
    }
}
