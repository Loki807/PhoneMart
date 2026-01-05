using Microsoft.EntityFrameworkCore;
using PhoneMart.Application.Contracts.Identity;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Domain.Enums;

namespace PhoneMart.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(IAppDbContext db, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<(string token, Guid userId, UserRole role)> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user is null || !user.IsActive)
            throw new Exception("Invalid email or password.");

        if (!_hasher.Verify(password, user.PasswordHash))
            throw new Exception("Invalid email or password.");

        var token = _jwt.GenerateToken(user.Id, user.Email, user.Role);
        return (token, user.Id, user.Role);
    }
}
