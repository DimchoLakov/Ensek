using Ensek.Api.Exceptions;
using System.Net;
using System.Text.Json;

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
        var result = string.Empty;

        switch (exception)
        {
            case ModelValidationException modelValidationException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    ValidationDetails = true,
                    Errors = modelValidationException.Errors
                });
                break;
            default:
                break;
        }

        context.Response.StatusCode = (int)httpStatusCode;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(result);
    }
}
