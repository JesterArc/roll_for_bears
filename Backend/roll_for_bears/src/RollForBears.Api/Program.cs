using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using roll_for_bears.Database;
using RollForBears.Api.MainModules.LoginModule.Services;
using RollForBears.Modules.Users;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(UsersModule).Assembly);

builder.Services.AddUsersModule(builder.Configuration);

builder.Services.AddScoped<IUserValidateService, UserValidateService>();

builder.Services.AddDbContext<RollForBearsContext>(options =>
    options.UseNpgsql(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ));

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();