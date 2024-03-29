﻿using CsvHelper;
using CsvHelper.TypeConversion;
using Ensek.Api.Exceptions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ensek.Api.Middlewares;

public class ExceptionValidationHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context, exception);
        }
    }

    private static Task HandleException(HttpContext context, Exception exception)
    {
        var httpStatusCode = HttpStatusCode.InternalServerError;
        var result = JsonSerializer.Serialize(
            new[] { "Something went wrong. Please try again later." },
            GetDefaultJsonSerializerOptions());

        switch (exception)
        {
            case ModelValidationException modelValidationException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(
                    new
                    {
                        ValidationDetails = true,
                        Errors = modelValidationException.Errors
                    },
                    GetDefaultJsonSerializerOptions());
                break;
            case HeaderValidationException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(
                    new[] { "File contains invalid Headers." },
                    GetDefaultJsonSerializerOptions());
                break;
            case ReaderException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(
                    new[] { $"Unable to read file. {exception.Message}" },
                    GetDefaultJsonSerializerOptions());
                break;
            case TypeConverterException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(
                    new[] { $"Unable to read file. {exception.Message}" },
                    GetDefaultJsonSerializerOptions());
                break;
            default:
                break;
        }

        context.Response.StatusCode = (int)httpStatusCode;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(result);
    }

    private static JsonSerializerOptions GetDefaultJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}
