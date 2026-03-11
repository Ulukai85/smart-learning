using Microsoft.EntityFrameworkCore;
using SmartLearning.Extensions;
using SmartLearning.Models;
using SmartLearning.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("AppSettings:Jwt"));
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("AppSettings:OpenAi"));

builder.Services
    .InjectServices()
    .AddEndpointExplorer()
    .InjectDbContext(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .ConfigureIdentityOptions()
    .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope()) 
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedAsync(services);
}

app.ConfigureEndpointExplorer();
app.ConfigureCors(builder.Configuration);
app.UseHttpsRedirection();
app.AddIdentityAuthMiddleware();

app.MapControllers();

app
    .MapGroup("api/Auth")
    .MapIdentityApi<AppUser>();

app.Run();