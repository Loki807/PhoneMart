using MediatR;

namespace PhoneMart.Application.Features.Products.Requests.Commands;

/// <summary>
/// COMMAND: "I want to delete a product"
/// 
/// IRequest<bool> means: returns true if deleted, false if not found.
/// 
/// OwnerUserId ensures only the product's owner can delete it.
/// Even if someone sends another owner's product ID, the handler
/// will check ownership and reject it.
/// </summary>
public class DeleteProductCommand : IRequest<bool>
{
    public Guid OwnerUserId { get; set; }   // From JWT token
    public Guid ProductId { get; set; }     // Which product to delete
}
