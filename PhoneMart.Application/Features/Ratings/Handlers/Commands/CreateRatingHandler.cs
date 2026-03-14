using MediatR;
using PhoneMart.Application.Contracts.Persistence;
using PhoneMart.Application.Features.Ratings.Requests.Commands;
using PhoneMart.Application.Features.Ratings.Validators;
using PhoneMart.Domain.Entities;

namespace PhoneMart.Application.Features.Ratings.Handlers.Commands;

/// <summary>
/// HANDLER: Creates or updates a shop rating.
/// 
/// UPSERT pattern:
///   - If the owner hasn't rated this shop → CREATE new rating
///   - If the owner already rated this shop → UPDATE existing rating
///   This allows owners to change their mind about a rating.
/// 
/// SECURITY:
///   - Can't rate your own shop
///   - RaterUserId from JWT (not from request body)
/// </summary>
public class CreateRatingHandler : IRequestHandler<CreateRatingCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateRatingHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
    {
        CreateRatingValidator.Validate(request);

        var dto = request.RatingDto;

        // Find the rater's shop
        var raterShop = _db.Shops.FirstOrDefault(s => s.OwnerUserId == request.RaterUserId);
        if (raterShop == null)
            throw new Exception("You don't have a shop.");

        // SECURITY: Can't rate your own shop
        if (raterShop.Id == dto.ShopId)
            throw new Exception("You cannot rate your own shop.");

        // Check the target shop exists
        var targetShop = _db.Shops.FirstOrDefault(s => s.Id == dto.ShopId);
        if (targetShop == null)
            throw new Exception("Shop not found.");

        // Check if already rated (upsert)
        var existing = _db.Ratings
            .FirstOrDefault(r => r.ShopId == dto.ShopId && r.RaterUserId == request.RaterUserId);

        if (existing != null)
        {
            // UPDATE existing rating
            existing.Stars = dto.Stars;
            existing.Comment = dto.Comment?.Trim();
            _db.UpdateEntity(existing);
            await _db.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        // CREATE new rating
        var rating = new Rating
        {
            Id = Guid.NewGuid(),
            ShopId = dto.ShopId,
            RaterUserId = request.RaterUserId,
            Stars = dto.Stars,
            Comment = dto.Comment?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _db.AddEntityAsync(rating, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return rating.Id;
    }
}
