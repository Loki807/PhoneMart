using MediatR;

namespace PhoneMart.Application.Features.Wholesale.Requests.Commands;

/// <summary>
/// COMMAND: "I want to delete my wholesale listing"
/// </summary>
public class DeleteWholesaleCommand : IRequest<bool>
{
    public Guid OwnerUserId { get; set; }
    public Guid ListingId { get; set; }
}
