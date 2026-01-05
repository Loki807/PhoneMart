using PhoneMart.Application.Features.Shops.Requests.Commands;

namespace PhoneMart.Application.Features.Shops.Validators;

public static class UpdateShopValidator
{
    public static void Validate(UpdateShopCommand request)
    {
        if (request.ShopDto == null)
            throw new Exception("ShopDto is required.");

        var dto = request.ShopDto;

        if (dto.ShopId == Guid.Empty)
            throw new Exception("ShopId is required.");

        if (string.IsNullOrWhiteSpace(dto.ShopName))
            throw new Exception("ShopName is required.");

        if (string.IsNullOrWhiteSpace(dto.City))
            throw new Exception("City is required.");

        if (string.IsNullOrWhiteSpace(dto.CallNumber))
            throw new Exception("CallNumber is required.");
    }
}
