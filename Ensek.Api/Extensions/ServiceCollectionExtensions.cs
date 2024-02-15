﻿using Ensek.Api.Validation;
using Ensek.Meters.Data;
using Ensek.Meters.Domain.Services;
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
        .AddTransient<IMeterService, MeterService>();
}