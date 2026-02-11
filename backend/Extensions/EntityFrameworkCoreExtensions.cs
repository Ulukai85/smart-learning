using Microsoft.EntityFrameworkCore;
using SmartLearning.Models;

namespace SmartLearning.Extensions;

public static class EntityFrameWorkCoreExtensions
{
    public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DevDB") ?? string.Empty;
        services.AddDbContext<AppDbContext>(options => 
            options.UseMySQL(connectionString));
        
        return services;
    }
}