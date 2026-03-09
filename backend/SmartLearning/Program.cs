using Microsoft.EntityFrameworkCore;
using SmartLearning.Extensions;
using SmartLearning.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("AppSettings:Jwt"));

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
    db.Database.Migrate();
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