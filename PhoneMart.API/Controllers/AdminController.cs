using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneMart.Application.Features.Shops.DTOs;
using PhoneMart.Application.Features.Shops.Requests.Commands;
using PhoneMart.Application.Features.Shops.Requests.Queries;

namespace PhoneMart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("shop-owners")]
    public async Task<IActionResult> CreateShopOwner([FromBody] CreateShopOwnerDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateShopOwnerCommand { ShopOwnerDto = dto };

        var shopId = await _mediator.Send(command, cancellationToken);

        return Ok(new { shopId });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("shops")]
    public async Task<IActionResult> GetAllShops(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllShopsQuery(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("shops/{id:guid}")]
    public async Task<IActionResult> GetShopById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetShopByIdQuery { Id = id }, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("shops")]
    public async Task<IActionResult> UpdateShop([FromBody] UpdateShopDto dto, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new UpdateShopCommand { ShopDto = dto }, cancellationToken);
        return Ok(new { shopId = id });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("shops/{id:guid}")]
    public async Task<IActionResult> DeleteShop(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _mediator.Send(new DeleteShopCommand { Id = id }, cancellationToken);
        if (!ok) return NotFound();
        return Ok(new { deleted = true });
    }
}
