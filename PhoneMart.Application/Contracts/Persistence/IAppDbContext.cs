using PhoneMart.Domain.Entities;

namespace PhoneMart.Application.Contracts.Persistence;

public interface IAppDbContext
{
    // ✅ Query access (no EF types here)
    IQueryable<User> Users { get; }
    IQueryable<Shop> Shops { get; }
    IQueryable<Category> Categories { get; }
    IQueryable<Brand> Brands { get; }
    IQueryable<Product> Products { get; }
    IQueryable<WholesaleListing> WholesaleListings { get; }

    // ✅ Generic helpers (Kaappaan style)
    Task AddEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    void UpdateEntity<T>(T entity) where T : class;
    Task<T?> FindAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class;
    void RemoveEntity<T>(T entity) where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
