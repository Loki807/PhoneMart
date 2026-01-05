using MediatR;
using PhoneMart.Application.Contracts.Identity;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Shops.Requests.Commands;
using PhoneMart.Application.Features.Shops.Validators;
using PhoneMart.Domain.Entities;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Shops.Handlers.Commands;

public class CreateShopOwnerHandler : IRequestHandler<CreateShopOwnerCommand, Guid>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;

    public CreateShopOwnerHandler(IAppDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<Guid> Handle(CreateShopOwnerCommand request, CancellationToken cancellationToken)
    {
        CreateShopOwnerValidator.Validate(request);

        var dto = request.ShopOwnerDto;

        var email = dto.Email.Trim().ToLower();

        // ✅ check email unique
        var emailExists = _db.Users.Any(u => u.Email.ToLower() == email);
        if (emailExists)
            throw new Exception("This email already exists.");

        // ✅ create owner user
        var ownerId = Guid.NewGuid();

        var owner = new User
        {
            Id = ownerId,
            FullName = dto.FullName.Trim(),
            Email = email,
            PasswordHash = _hasher.Hash(dto.Password),
            Role = UserRole.Owner,
            IsActive = true
        };

        await _db.AddEntityAsync(owner, cancellationToken);

        // ✅ create shop (MATCHES YOUR Shop entity)
        var shopId = Guid.NewGuid();

        var shop = new Shop
        {
            Id = shopId,
            OwnerUserId = ownerId,
            ShopName = dto.ShopName.Trim(),
            Address = dto.Address?.Trim(),
            City = dto.City.Trim(),
            WhatsAppNumber = dto.WhatsAppNumber?.Trim() ?? "",
            CallNumber = dto.PhoneNumber.Trim(),
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await _db.AddEntityAsync(shop, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return shopId;
    }
}
