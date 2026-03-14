using MediatR;
using PhoneMart.Application.Features.Wholesale.DTOs;

namespace PhoneMart.Application.Features.Wholesale.Requests.Commands;

/// <summary>
/// COMMAND: "I want to create a new wholesale listing"
/// Same pattern as CreateProductCommand — OwnerUserId from JWT.
/// </summary>
public class CreateWholesaleCommand : IRequest<Guid>
{
    public Guid OwnerUserId { get; set; }
    public CreateWholesaleDto WholesaleDto { get; set; } = default!;
}
