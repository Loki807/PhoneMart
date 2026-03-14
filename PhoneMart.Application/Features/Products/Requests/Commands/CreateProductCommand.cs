using MediatR;
using PhoneMart.Application.Features.Products.DTOs;

namespace PhoneMart.Application.Features.Products.Requests.Commands;

/// <summary>
/// COMMAND: "I want to create a new product"
/// 
/// This is a MediatR message. When the controller sends this,
/// MediatR automatically finds CreateProductHandler and runs it.
/// 
/// IRequest<Guid> means: "After processing, return a Guid (the new product ID)"
/// 
/// Notice: OwnerUserId is included here (set by the controller from JWT token).
/// The DTO doesn't have it, but the Command does. This is the secure way —
/// the controller extracts the user ID from the JWT, not from the request body.
/// </summary>
public class CreateProductCommand : IRequest<Guid>
{
    public Guid OwnerUserId { get; set; }            // From JWT token (secure)
    public CreateProductDto ProductDto { get; set; } = default!;  // From request body
}
