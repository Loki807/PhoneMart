using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhoneMart.API.Middleware;
using PhoneMart.Application;
using PhoneMart.Infrastructure;
using PhoneMart.Persistence;
using System.Text;

namespace PhoneMart.API;

/// <summary>
/// APPLICATION ENTRY POINT
/// 
/// This file configures:
///   1. Service Registration (DI containers)
///   2. JWT Authentication
///   3. CORS (Cross-Origin Resource Sharing)
///   4. Swagger (API documentation)
///   5. Error Handling Middleware
///   6. Database Seeding (Admin + Brands)
///   7. Request Pipeline (middleware order matters!)
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  1. CORE SERVICES
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        builder.Services.AddControllers();

        // Application layer (MediatR handlers)
        builder.Services.AddApplicationServices();

        // Persistence layer (EF Core + Database)
        builder.Services.AddPersistenceServices(builder.Configuration);

        // Infrastructure layer (JWT, Password Hashing, AWS S3)
        builder.Services.AddInfrastructureServices(builder.Configuration);

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  2. JWT AUTHENTICATION
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
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

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  3. CORS — Allow frontend to call backend
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //
        // Without CORS: Browser blocks frontend (localhost:3000)
        // from calling backend (localhost:5236).
        // CORS tells the browser: "It's OK, I trust this origin."
        //
        var corsOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? new[] { "http://localhost:3000" };

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(corsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  4. SWAGGER — API Documentation
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //
        // Swagger generates a web page with all your API endpoints.
        // Access it at: http://localhost:5236/swagger
        // You can test endpoints directly from the browser!
        //
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PhoneMart API",
                Version = "v1",
                Description = "Backend API for PhoneMart — Sri Lanka's phone shop marketplace"
            });

            // Add JWT auth support in Swagger UI
            // This adds the "Authorize" button so you can test protected endpoints
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  BUILD THE APP
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        var app = builder.Build();

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  5. SEED DATABASE (Admin + Brands)
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PhoneMart.Persistence.Data.AppDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<PhoneMart.Application.Contracts.Identity.IPasswordHasher>();

            // Auto-create database & apply all migrations (needed for fresh RDS)
            await db.Database.MigrateAsync();

            await PhoneMart.Persistence.Seeds.AdminSeedData.SeedAsync(db, hasher);
            await PhoneMart.Persistence.Seeds.BrandSeedData.SeedAsync(db);
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  6. REQUEST PIPELINE (order matters!)
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //
        // Middleware order:
        //   Error Handling → CORS → Swagger → Auth → Controllers
        //
        // Error middleware must be FIRST so it catches errors from everything below.
        // CORS must be before Auth so preflight requests don't get blocked.
        //

        // Error handling — catches all exceptions, returns clean JSON
        app.UseMiddleware<ErrorHandlingMiddleware>();

        // Swagger — only in development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // CORS
        app.UseCors("AllowFrontend");

        // Static files — serve uploaded images from wwwroot/uploads/
        var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsPath);
        app.UseStaticFiles(); // Serves files from wwwroot/

        // Auth
        app.UseAuthentication();
        app.UseAuthorization();

        // Controllers
        app.MapControllers();

        app.Run();
    }
}
