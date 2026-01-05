namespace PhoneMart.Application.Features.Shops.DTOs;

public class UpdateShopDto
{
    public Guid ShopId { get; set; }

    public string ShopName { get; set; } = "";
    public string? Address { get; set; }
    public string City { get; set; } = "";

    public string WhatsAppNumber { get; set; } = "";
    public string CallNumber { get; set; } = "";

    public bool IsVerified { get; set; }
}
