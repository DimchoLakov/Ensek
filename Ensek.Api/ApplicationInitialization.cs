using Ensek.Meters.Domain.Services.Initializers;
using Ensek.Meters.Domain.Services.Seeders;

namespace Ensek.Api;

public static class ApplicationInitialization
{
    public static IApplicationBuilder Initialize(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        var initializers = serviceProvider.GetServices<IInitializer>();
        foreach (var initializer in initializers)
        {
            initializer.Initialize();
        }

        var dataSeeders = serviceProvider.GetServices<IDataSeeder>();
        foreach (var dataSeeder in dataSeeders)
        {
            dataSeeder.Seed();
        }

        return app;
    }
}
