using Microsoft.EntityFrameworkCore;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Domain.Entities;

namespace PhoneMart.Persistence.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ✅ EF Core DbSets (only in Persistence)
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Shop> Shops { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Brand> Brands { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<WholesaleListing> WholesaleListings { get; set; } = default!;

    // ✅ OTP DbSet
    public DbSet<EmailOtp> EmailOtps { get; set; } = default!;

    // ✅ Interface exposes IQueryable (Kaappaan style)
    IQueryable<User> IAppDbContext.Users => Users;
    IQueryable<Shop> IAppDbContext.Shops => Shops;
    IQueryable<Category> IAppDbContext.Categories => Categories;
    IQueryable<Brand> IAppDbContext.Brands => Brands;
    IQueryable<Product> IAppDbContext.Products => Products;
    IQueryable<WholesaleListing> IAppDbContext.WholesaleListings => WholesaleListings;
    IQueryable<EmailOtp> IAppDbContext.EmailOtps => EmailOtps;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ✅ Unique email
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        // ✅ Owner(User) 1 - 1 Shop
        modelBuilder.Entity<User>()
            .HasOne(u => u.Shop)
            .WithOne(s => s.OwnerUser)
            .HasForeignKey<Shop>(s => s.OwnerUserId);

        // ✅ Shop 1 - many Products
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Shop)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.ShopId);

        // ✅ Shop 1 - many WholesaleListings
        modelBuilder.Entity<WholesaleListing>()
            .HasOne(w => w.SellerShop)
            .WithMany(s => s.WholesaleListings)
            .HasForeignKey(w => w.SellerShopId);

        // ✅ Decimal precision (avoid truncation in SQL Server)
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<WholesaleListing>()
            .Property(w => w.UnitPrice)
            .HasPrecision(18, 2);

        // ✅ OTP table configuration (IMPORTANT)
        modelBuilder.Entity<EmailOtp>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(10);

            // ✅ Index for fast lookup
            entity.HasIndex(x => x.Email);
            entity.HasIndex(x => new { x.Email, x.Code });

            // optional: default false
            entity.Property(x => x.IsUsed)
                .HasDefaultValue(false);
        });

        // ✅ Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Used Phones" },
            new Category { Id = 2, Name = "New Phones" },
            new Category { Id = 3, Name = "Accessories" }
        );

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);

    public void UpdateEntity<T>(T entity) where T : class
        => Set<T>().Update(entity);

    public async Task AddEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        => await Set<T>().AddAsync(entity, cancellationToken);

    public async Task<T?> FindAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class
        => await Set<T>().FindAsync(new object[] { id }, cancellationToken);

    public void RemoveEntity<T>(T entity) where T : class
        => Set<T>().Remove(entity);
}
