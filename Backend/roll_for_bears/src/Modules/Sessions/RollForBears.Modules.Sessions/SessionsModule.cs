using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
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
            options.UseNpgsql(connectionString));
        
        services.AddDbContext<SessionsDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                    npgsqlOptions
                        .MapEnum<GameType>("game_type", "session_info", new EnumTranslator())
                        .MapEnum<PlayerRole>("player_role", "session_info", new EnumTranslator())
            ));

        services.AddScoped<ISessionsApi, SessionsService>();
        
        //TODO: Authentication and Authorization
        
        return services;
    }
}

class EnumTranslator : INpgsqlNameTranslator
{
    public string TranslateTypeName(string clrName)
    {
        return string.Concat(clrName[0].ToString().ToUpper(), clrName[1..]);
    }

    public string TranslateMemberName(string clrName)
    {
        return clrName.ToLower();
    }
}