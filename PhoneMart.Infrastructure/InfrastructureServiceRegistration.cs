using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhoneMart.Application.Contracts.Email;
using PhoneMart.Application.Contracts.Identity;
using PhoneMart.Application.Contracts.OTP;
using PhoneMart.Infrastructure.Identity;

namespace PhoneMart.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailOtpService, EmailOtpService>();

        return services;
    }
}
