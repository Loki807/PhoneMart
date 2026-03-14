using PhoneMart.Application.Features.Wholesale.Requests.Commands;

namespace PhoneMart.Application.Features.Wholesale.Validators;

/// <summary>
/// VALIDATOR: Checks wholesale input before processing.
/// 
/// Business Rules:
///   - ItemType must be valid (1-4)
///   - Title required
///   - UnitPrice must be positive
///   - MinOrderQty must be at least 1
///   - AvailableQty must be >= MinOrderQty (can't sell less than minimum)
/// </summary>
public static class CreateWholesaleValidator
{
    public static void Validate(CreateWholesaleCommand request)
    {
        if (request.WholesaleDto == null)
            throw new Exception("WholesaleDto is required.");

        var dto = request.WholesaleDto;

        if (dto.ItemType < 1 || dto.ItemType > 4)
            throw new Exception("ItemType must be 1 (Phone), 2 (Accessory), 3 (SparePart), or 4 (RepairItem).");

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new Exception("Title is required.");

        if (dto.Title.Length > 200)
            throw new Exception("Title must be 200 characters or less.");

        if (dto.UnitPrice <= 0)
            throw new Exception("UnitPrice must be greater than 0.");

        if (dto.MinOrderQty < 1)
            throw new Exception("MinOrderQty must be at least 1.");

        if (dto.AvailableQty < 1)
            throw new Exception("AvailableQty must be at least 1.");

        if (dto.AvailableQty < dto.MinOrderQty)
            throw new Exception("AvailableQty must be >= MinOrderQty.");
    }
}
