using MediatR;
using PhoneMart.Application.Features.Public.DTOs;

namespace PhoneMart.Application.Features.Public.Requests.Queries;

/// <summary>
/// QUERY: "Show me all products in a specific shop"
/// Only returns ACTIVE products (not Sold/Hidden).
/// Optional category filter (1=Used, 2=New, 3=Accessories).
/// </summary>
public class GetPublicShopProductsQuery : IRequest<List<PublicProductDto>>
{
    public Guid ShopId { get; set; }
    public int? CategoryFilter { get; set; }   // Optional: filter by category
}
