using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Sessions.Database;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Services;

namespace RollForBears.Modules.Sessions;

public static class SessionsModule
{
    public static IServiceCollection AddSessionsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DB_CONNECTION_STRING"] ?? throw new InvalidOperationException(
            "DB_CONNECTION_STRING is not configured.");
        
        services.AddDbContext<SessionsDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                    npgsqlOptions
                        .MapEnum<GameType>("game_type", "gameplay")
                        .MapEnum<PlayerRole>("player_role", "gameplay")
            ));

        services.AddScoped<ISessionsApi, SessionsService>();
        
        //TODO: Authentication and Authorization

        try
        {
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                            ?? throw new InvalidOperationException("JWT_ISSUER is not defined");

            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                              ?? throw new InvalidOperationException("JWT_AUDIENCE is not defined");

            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                            ?? throw new InvalidOperationException("JWT_SECRET is not defined");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("sessionAuth", options =>
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
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }

        services.AddAuthorization();
        
        return services;
    }
}