using PhoneMart.Application.Features.Wholesale.Requests.Commands;

namespace PhoneMart.Application.Features.Wholesale.Validators;

/// <summary>
/// VALIDATOR: Checks wholesale update input.
/// Same rules as create, plus ListingId and Status checks.
/// </summary>
public static class UpdateWholesaleValidator
{
    public static void Validate(UpdateWholesaleCommand request)
    {
        if (request.WholesaleDto == null)
            throw new Exception("WholesaleDto is required.");

        var dto = request.WholesaleDto;

        if (dto.ListingId == Guid.Empty)
            throw new Exception("ListingId is required.");

        if (dto.ItemType < 1 || dto.ItemType > 4)
            throw new Exception("ItemType must be 1-4.");

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new Exception("Title is required.");

        if (dto.UnitPrice <= 0)
            throw new Exception("UnitPrice must be greater than 0.");

        if (dto.MinOrderQty < 1)
            throw new Exception("MinOrderQty must be at least 1.");

        if (dto.AvailableQty < 0)
            throw new Exception("AvailableQty cannot be negative.");

        if (dto.Status < 1 || dto.Status > 3)
            throw new Exception("Status must be 1 (Active), 2 (Sold), or 3 (Hidden).");
    }
}
