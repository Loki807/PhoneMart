using MediatR;
using PhoneMart.Application.Features.Products.DTOs;

namespace PhoneMart.Application.Features.Products.Requests.Queries;

/// <summary>
/// QUERY: "Give me a single product by its ID"
/// 
/// IRequest<ProductDto?> — the ? means it could return null
/// (if the product doesn't exist).
/// </summary>
public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public Guid ProductId { get; set; }
}
