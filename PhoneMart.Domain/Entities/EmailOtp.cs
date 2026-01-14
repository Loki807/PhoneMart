namespace PhoneMart.Domain.Entities;

public class EmailOtp
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // 6 digit
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsUsed { get; set; } = false;
}
