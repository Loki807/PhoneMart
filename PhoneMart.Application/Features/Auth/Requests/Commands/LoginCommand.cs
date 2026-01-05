using MediatR;
using PhoneMart.Application.Features.Auth.DTOs;

namespace PhoneMart.Application.Features.Auth.Requests.Commands;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
