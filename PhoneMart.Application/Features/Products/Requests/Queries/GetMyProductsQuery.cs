using MediatR;
using PhoneMart.Application.Features.Products.DTOs;

namespace PhoneMart.Application.Features.Products.Requests.Queries;

/// <summary>
/// QUERY: "Give me all products for MY shop"
/// 
/// This is a READ operation — it doesn't change any data.
/// The OwnerUserId is used to find the owner's shop, 
/// then return only that shop's products.
/// 
/// IRequest<List<ProductDto>> means: returns a list of products.
/// </summary>
public class GetMyProductsQuery : IRequest<List<ProductDto>>
{
    public Guid OwnerUserId { get; set; }   // From JWT token
}
