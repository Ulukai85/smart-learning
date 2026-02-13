namespace SmartLearning.Extensions;

public static class AppConfigExtensions
{
    public static WebApplication ConfigureCors(this WebApplication app, IConfiguration config)
    {
        app.UseCors(options =>
            options.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader());
        return app;
    }
}