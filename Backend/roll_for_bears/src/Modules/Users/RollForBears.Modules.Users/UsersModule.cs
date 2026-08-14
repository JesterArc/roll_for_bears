using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Database;
using RollForBears.Modules.Users.Services;

namespace RollForBears.Modules.Users;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DB_CONNECTION_STRING"] ?? throw new InvalidOperationException(
            "DB_CONNECTION_STRING is not configured.");

        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUsersApi, UsersService>();

        return services;
    }
}