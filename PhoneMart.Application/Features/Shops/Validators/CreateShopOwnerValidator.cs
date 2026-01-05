using PhoneMart.Application.Features.Shops.Requests.Commands;

namespace PhoneMart.Application.Features.Shops.Validators;

public static class CreateShopOwnerValidator
{
    public static void Validate(CreateShopOwnerCommand request)
    {
        if (request.ShopOwnerDto == null)
            throw new Exception("ShopOwnerDto is required.");

        var dto = request.ShopOwnerDto;

        if (string.IsNullOrWhiteSpace(dto.FullName))
            throw new Exception("FullName is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new Exception("Email is required.");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            throw new Exception("Password must be at least 6 characters.");

        if (string.IsNullOrWhiteSpace(dto.ShopName))
            throw new Exception("ShopName is required.");

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            throw new Exception("PhoneNumber is required.");

        if (string.IsNullOrWhiteSpace(dto.Address))
            throw new Exception("Address is required.");

        if (string.IsNullOrWhiteSpace(dto.City))
            throw new Exception("City is required.");
    }
}
