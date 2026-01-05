namespace PhoneMart.Application.Features.Shops.DTOs;

public class CreateShopOwnerDto
{
    // Owner
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    // Shop
    public string ShopName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string WhatsAppNumber { get; set; } = "";
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
}
