using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RollForBears.Modules.RpgContent.Contracts.Api;
using RollForBears.Modules.RpgContent.Database;
using RollForBears.Modules.RpgContent.Services;

namespace RollForBears.Modules.RpgContent;

public static class RpgContentModule
{
    public static IServiceCollection AddRpgContentModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DB_CONNECTION_STRING"] ?? throw new InvalidOperationException(
            "DB_CONNECTION_STRING is not configured.");
        
        services.AddDbContext<RpgContentDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IRpgContentApi, RpgContentService>();
        
        return services;
    }
}