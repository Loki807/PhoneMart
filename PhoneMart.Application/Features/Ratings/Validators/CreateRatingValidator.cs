using PhoneMart.Application.Features.Ratings.Requests.Commands;

namespace PhoneMart.Application.Features.Ratings.Validators;

/// <summary>
/// VALIDATOR: Checks rating input.
/// Business rules: Stars 1-5, ShopId required, Comment max 500 chars.
/// </summary>
public static class CreateRatingValidator
{
    public static void Validate(CreateRatingCommand request)
    {
        if (request.RatingDto == null)
            throw new Exception("RatingDto is required.");

        var dto = request.RatingDto;

        if (dto.ShopId == Guid.Empty)
            throw new Exception("ShopId is required.");

        if (dto.Stars < 1 || dto.Stars > 5)
            throw new Exception("Stars must be between 1 and 5.");

        if (dto.Comment != null && dto.Comment.Length > 500)
            throw new Exception("Comment must be 500 characters or less.");
    }
}
