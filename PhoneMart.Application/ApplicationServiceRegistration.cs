using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PhoneMart.Application.Features.Auth.Handlers.Commands;

namespace PhoneMart.Application;

public static class ApplicationServiceRegistrationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ✅ Old MediatR registration (for older versions)
        services.AddMediatR(typeof(LoginHandler).Assembly);

        return services;
    }
}
