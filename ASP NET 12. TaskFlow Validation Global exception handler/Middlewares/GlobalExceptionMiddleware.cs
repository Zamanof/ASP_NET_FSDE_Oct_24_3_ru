using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception occured while processing request");

        ProblemDetails problem;
        int statusCode;

        switch (ex)
        {
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                problem = CreateValidationProblemDetails(context, validationException, statusCode);
                break;
            case KeyNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                problem = CreateProblemDetails(context, statusCode, "Resource not found", ex.Message);
                break;
            case ArgumentException:
                statusCode = (int)HttpStatusCode.BadRequest;
                problem = CreateProblemDetails(context, statusCode, "Invalid Request", ex.Message);
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                problem = CreateProblemDetails(context, statusCode, "An unxpected error occured", "An unxpected error occured while processing request");
                break;
        }

        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, int statusCode, string title, string detail)
    {
        return new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };
    }

    private ProblemDetails CreateValidationProblemDetails(HttpContext context, ValidationException validationException, int statusCode)
    {
        var errors = validationException.Errors
                                            .GroupBy(e => e.PropertyName)
                                            .ToDictionary(
                                                    g => g.Key,
                                                    g => g.Select(e => e.ErrorMessage)
                                                    .ToArray()
                                                    );
        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
            Title = "One or more validation error occured",
            Status = statusCode,
            Detail = "See the 'errors' property for more details",
            Instance = context.Request.Path
        };
        problem.Extensions["errors"] = errors;
        return problem;
    }
}
