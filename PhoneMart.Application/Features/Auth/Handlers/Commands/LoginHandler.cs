using MediatR;
using PhoneMart.Application.Contracts.Identity;
using PhoneMart.Application.Features.Auth.DTOs;
using PhoneMart.Application.Features.Auth.Requests.Commands;

namespace PhoneMart.Application.Features.Auth.Handlers.Commands;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAuthService _authService;

    public LoginHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (token, userId, role) = await _authService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return new LoginResponseDto
        {
            Token = token,
            UserId = userId,
            Role = role
        };
    }
}
