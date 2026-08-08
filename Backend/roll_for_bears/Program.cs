using Microsoft.EntityFrameworkCore;
using roll_for_bears.Database;
using roll_for_bears.MainModules.LoginModule.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IUserValidateService, UserValidateService>();

DotNetEnv.Env.Load();

builder.Services.AddDbContext<RollForBearsContext>(options => 
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")));
    
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();