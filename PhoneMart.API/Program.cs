using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PhoneMart.Application;
using PhoneMart.Infrastructure;
using PhoneMart.Persistence;
using System.Text;

namespace PhoneMart.API;

public class Program
{
    public static async Task Main(string[] args)   // ✅ change: async Task
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        // ✅ Application CQRS registrations
        builder.Services.AddApplicationServices();

        // ✅ Persistence + Infrastructure
        builder.Services.AddPersistenceServices(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);

        // ✅ JWT Auth
        var jwtKey = builder.Configuration["Jwt:Key"]!;
        var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
        var jwtAudience = builder.Configuration["Jwt:Audience"]!;

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PhoneMart.Persistence.Data.AppDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<PhoneMart.Application.Contracts.Identity.IPasswordHasher>();

            await PhoneMart.Persistence.Seeds.AdminSeedData.SeedAsync(db, hasher); // ✅ change: await
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
