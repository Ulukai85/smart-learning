using SmartLearning.Repositories;
using SmartLearning.Services;

namespace SmartLearning.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection InjectServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDeckService, DeckService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IReviewService, ReviewService>();

        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<IDeckRepository, DeckRepository>();
        
        return services;
    }
}