using roll_for_bears.MainModules.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IUserValidateService, UserValidateService>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();