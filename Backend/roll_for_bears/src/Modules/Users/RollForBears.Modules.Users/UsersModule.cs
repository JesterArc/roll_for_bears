using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Database;
using RollForBears.Modules.Users.Models;
using RollForBears.Modules.Users.Security;
using RollForBears.Modules.Users.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace RollForBears.Modules.Users;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DB_CONNECTION_STRING"] ?? throw new InvalidOperationException(
            "DB_CONNECTION_STRING is not configured.");

        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                    npgsqlOptions.MapEnum<AccountStatus>("status")
            ));

        services.AddScoped<IUsersApi, UsersService>();
        
        string pepper = Environment.GetEnvironmentVariable("PASSWORD_PEPPER")
            ?? throw new InvalidOperationException("PASSWORD_PEPPER is not defined");
        
        services.AddSingleton(new PasswordHasher(pepper));
        
        string jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? throw new InvalidOperationException("JWT_ISSUER is not defined");
        
        string jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? throw new InvalidOperationException("JWT_AUDIENCE is not defined");
        
        string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new InvalidOperationException("JWT_SECRET is not defined");
        
        services.AddSingleton(new JwtTokenGenerator(jwtSecret, jwtIssuer, jwtAudience));

        services.AddSingleton(new RefreshTokenGenerator());

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,

                    ValidateAudience = true,
                    ValidAudience = jwtAudience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Convert.FromBase64String(jwtSecret)
                    ),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        
        return services;
    }
}