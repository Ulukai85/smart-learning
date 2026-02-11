using SmartLearning.Services;

namespace SmartLearning.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection InjectServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}