using PhoneMart.Domain.Entities;

namespace PhoneMart.Application.Contracts.Persistence;

public interface IAppDbContext
{
    IQueryable<User> Users { get; }
    IQueryable<Shop> Shops { get; }
    IQueryable<Category> Categories { get; }
    IQueryable<Brand> Brands { get; }
    IQueryable<Product> Products { get; }
    IQueryable<WholesaleListing> WholesaleListings { get; }

    // ✅ ADD THIS for OTP
    IQueryable<EmailOtp> EmailOtps { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task AddEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    Task<T?> FindAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class;
    void UpdateEntity<T>(T entity) where T : class;
    void RemoveEntity<T>(T entity) where T : class;
}
