using MediatR;
using PhoneMart.Application.Features.Products.DTOs;

namespace PhoneMart.Application.Features.Products.Requests.Commands;

/// <summary>
/// COMMAND: "I want to update an existing product"
/// 
/// OwnerUserId is used inside the handler to verify that
/// the product actually belongs to this owner's shop.
/// </summary>
public class UpdateProductCommand : IRequest<Guid>
{
    public Guid OwnerUserId { get; set; }            // From JWT token
    public UpdateProductDto ProductDto { get; set; } = default!;
}
