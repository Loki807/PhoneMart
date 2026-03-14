namespace PhoneMart.Domain.Entities;

public class Shop
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Relationship: One Owner(User) -> One Shop
    public Guid OwnerUserId { get; set; }
    public User OwnerUser { get; set; } = default!;

    public string ShopName { get; set; } = "";
    public string? Address { get; set; }
    public string City { get; set; } = "Jaffna";

    // Contacts for WhatsApp + Call buttons
    public string WhatsAppNumber { get; set; } = "";
    public string CallNumber { get; set; } = "";

    public bool IsVerified { get; set; } = false;
    public string? ImageUrl { get; set; } // Shop logo or photo
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationship: One Shop -> Many Products
    public List<Product> Products { get; set; } = new();

    // Relationship: One Shop -> Many WholesaleListings
    public List<WholesaleListing> WholesaleListings { get; set; } = new();
}
