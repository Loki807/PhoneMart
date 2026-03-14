using MediatR;
using PhoneMart.Application.Features.Public.DTOs;

namespace PhoneMart.Application.Features.Public.Requests.Queries;

/// <summary>
/// QUERY: "Search products across ALL shops"
/// 
/// Business: Customer types "iPhone 15" → sees results from every shop.
/// Only Active products shown. Can filter by category and city.
/// </summary>
public class SearchProductsQuery : IRequest<List<PublicProductDto>>
{
    public string? SearchTerm { get; set; }      // "iPhone 15"
    public int? CategoryFilter { get; set; }     // 1=Used, 2=New, 3=Accessories
    public string? CityFilter { get; set; }      // "Jaffna"
}
