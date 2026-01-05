using PhoneMart.Domain.Enums;

namespace PhoneMart.Application.Features.Auth.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = "";
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
}
