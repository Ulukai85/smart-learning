namespace SmartLearning.Extensions;

public static class EndpointExplorerExtensions
{
    public static IServiceCollection AddEndpointExplorer(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static WebApplication ConfigureEndpointExplorer(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return app;
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "api");
        });
        return app;
    }
}