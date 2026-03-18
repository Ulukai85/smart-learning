using SmartLearning.Repositories;
using SmartLearning.Services;
using SmartLearning.SpacedRepetition;

namespace SmartLearning.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection InjectServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDeckService, DeckService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IStatisticService, StatisticService>();
        services.AddScoped<ISpacedRepetitionStrategy, AnkiSpacedRepetition>();
        services.AddScoped<ISpacedRepetitionFactory, SpacedRepetitionFactory>();

        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<IDeckRepository, DeckRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        
        services.AddScoped<IAiService, OpenAiService>();
        
        return services;
    }
}