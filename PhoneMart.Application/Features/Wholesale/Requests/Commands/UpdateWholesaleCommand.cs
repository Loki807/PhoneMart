using MediatR;
using PhoneMart.Application.Features.Wholesale.DTOs;

namespace PhoneMart.Application.Features.Wholesale.Requests.Commands;

/// <summary>
/// COMMAND: "I want to update my wholesale listing"
/// </summary>
public class UpdateWholesaleCommand : IRequest<Guid>
{
    public Guid OwnerUserId { get; set; }
    public UpdateWholesaleDto WholesaleDto { get; set; } = default!;
}
