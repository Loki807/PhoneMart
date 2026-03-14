using Microsoft.EntityFrameworkCore;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Domain.Entities;

namespace PhoneMart.Persistence.Seeds;

/// <summary>
/// SEED DATA: Populates the Brand table with common phone brands.
/// 
/// Runs on application startup (same as AdminSeedData).
/// Only adds brands that don't already exist (safe to run multiple times).
/// 
/// Why seed brands?
///   - Consistent data across all environments (dev, staging, production)
///   - Frontend can show brand dropdown immediately
///   - No manual database setup needed
/// </summary>
public static class BrandSeedData
{
    public static async Task SeedAsync(IAppDbContext db)
    {
        var existingCount = db.Brands.Count();
        if (existingCount > 0) return;  // Already seeded

        var brands = new List<Brand>
        {
            new() { Name = "Apple" },
            new() { Name = "Samsung" },
            new() { Name = "Huawei" },
            new() { Name = "Xiaomi" },
            new() { Name = "Oppo" },
            new() { Name = "Vivo" },
            new() { Name = "Realme" },
            new() { Name = "OnePlus" },
            new() { Name = "Nokia" },
            new() { Name = "Google" },
            new() { Name = "Sony" },
            new() { Name = "Motorola" },
            new() { Name = "Other" }
        };

        foreach (var brand in brands)
        {
            await db.AddEntityAsync(brand, CancellationToken.None);
        }

        await db.SaveChangesAsync(CancellationToken.None);
    }
}
