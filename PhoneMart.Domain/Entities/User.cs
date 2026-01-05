using PhoneMart.Domain.Enums;

namespace PhoneMart.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }

    // Stored hashed password (never store plain password)
    public string PasswordHash { get; set; } = "";

    public UserRole Role { get; set; } = UserRole.Owner;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationship: Owner has exactly one Shop (Admin has none)
    public Shop? Shop { get; set; }
}
