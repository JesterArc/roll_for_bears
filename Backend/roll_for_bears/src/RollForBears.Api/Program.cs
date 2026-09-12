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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("Angular");

app.UseAuthorization();

app.MapControllers();

app.Run();