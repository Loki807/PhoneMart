using Microsoft.EntityFrameworkCore;
using PhoneMart.Domain.Entities;
using PhoneMart.Domain.Enums;
using PhoneMart.Persistence.Data;
using PhoneMart.Application.Contracts.Identity;

namespace PhoneMart.Persistence.Seeds;

public static class AdminSeedData
{
    public static async Task SeedAsync(
        AppDbContext db,
        IPasswordHasher passwordHasher)
    {
        // ✅ check admin already exists
        var adminEmail = "admin@phonemart.lk";

        var exists = await db.Users.AnyAsync(x => x.Email == adminEmail);
        if (exists)
            return;

        // ✅ create admin
        var admin = new User
        {
            FullName = "PhoneMart Admin",
            Email = adminEmail,
            Role = UserRole.Admin,
            PasswordHash = passwordHasher.Hash("Admin@12345"),
            IsActive = true
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
