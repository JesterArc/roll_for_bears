using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Sessions.Database;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Services;

namespace RollForBears.Modules.Sessions;

public static class SessionModule
{
    public static IServiceCollection AddSessionModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DB_CONNECTION_STRING"] ?? throw new InvalidOperationException(
            "DB_CONNECTION_STRING is not configured.");
        
        services.AddDbContext<SessionsDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddDbContext<SessionsDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                    npgsqlOptions.MapEnum<GameType>("game_type").MapEnum<PlayerRole>("player_role")
            ));

        services.AddScoped<ISessionsApi, SessionsService>();
        
        //TODO: Authentication and Authorization
        
        return services;
    }
}