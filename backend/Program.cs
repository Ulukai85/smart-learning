using SmartLearning.Extensions;
using SmartLearning.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .InjectServices()
    .AddEndpointExplorer()
    .InjectDbContext(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .ConfigureIdentityOptions()
    .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

app.ConfigureEndpointExplorer();
app.ConfigureCors(builder.Configuration);
app.UseHttpsRedirection();
app.AddIdentityAuthMiddleware();

app.MapControllers();

app
    .MapGroup("api/Auth")
    .MapIdentityApi<AppUser>();

app.Run();