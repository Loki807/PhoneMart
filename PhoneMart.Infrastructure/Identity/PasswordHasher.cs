using System.Security.Cryptography;
using PhoneMart.Application.Contracts.Identity;

namespace PhoneMart.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    // PBKDF2 parameters (safe default)
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public string Hash(string plainPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            plainPassword,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        var key = pbkdf2.GetBytes(KeySize);

        // Format: iterations.salt.key  (Base64)
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string plainPassword, string passwordHash)
    {
        var parts = passwordHash.Split('.', 3);
        if (parts.Length != 3) return false;

        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            plainPassword,
            salt,
            iterations,
            HashAlgorithmName.SHA256);

        var keyToCheck = pbkdf2.GetBytes(KeySize);
        return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
    }
}
