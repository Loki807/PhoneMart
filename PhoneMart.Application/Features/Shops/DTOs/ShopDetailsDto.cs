namespace PhoneMart.Application.Features.Shops.DTOs;

public class ShopDetailsDto
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public string OwnerEmail { get; set; } = "";

    public string ShopName { get; set; } = "";
    public string? Address { get; set; }
    public string City { get; set; } = "";

    public string WhatsAppNumber { get; set; } = "";
    public string CallNumber { get; set; } = "";

    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}
