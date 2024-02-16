using Ensek.Api.Validation;
using Ensek.Meters.Data;
using Ensek.Meters.Data.Repositories.Accounts;
using Ensek.Meters.Data.Repositories.MeterReadings;
using Ensek.Meters.Domain.Services.Csv;
using Ensek.Meters.Domain.Services.Initializers;
using Ensek.Meters.Domain.Services.Meters;
using Ensek.Meters.Domain.Services.Seeders;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddDbContext<EnsekDbContext>(options => options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

    public static IServiceCollection AddApplicationServices(this IServiceCollection services) =>
        services
        .AddTransient<IRequestValidationService, RequestValidationService>()
        .AddTransient<IMeterService, MeterService>()
        .AddTransient<ICsvReaderService, CsvReaderService>()
        .AddTransient<IInitializer, DatabaseInitializer>()
        .AddTransient<IDataSeeder, DatabaseSeeder>()
        .AddScoped<IAccountRepository, AccountRepository>()
        .AddScoped<IMeterReadingRepository, MeterReadingRepository>();
}
