using Ensek.Api.Extensions;
using Ensek.Api.Middlewares;
using Ensek.Api.Validation.Validators;
using Ensek.Meters.Domain.MappingProfiles;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ensek.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register services
        builder
            .Services
            .AddControllers();

        builder
            .Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddDatabase(builder.Configuration)
            .AddValidatorsFromAssemblyContaining<UploadMeterReadingsRequestValidator>()
            .AddAutoMapper(typeof(EnsekMappingProfile))
            .AddApplicationServices();

        builder
            .Services
            .Configure<ApiBehaviorOptions>(config => config.SuppressModelStateInvalidFilter = true);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection()
           .UseRouting();

        app.UseMiddleware<ExceptionValidationHandlerMiddleware>();

        app.MapControllers();

        app.Initialize();

        app.Run();
    }
}
