using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ASP_NET_17._TaskFlow_Ownership.Middlewares;

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

        context.Response.ContentType = "application/json+problem";


        var (statusCode, problem) = ex switch
        {
            ValidationException validationException
            => (400, CreateValidationProblemDetails(context, validationException, 400)),

            KeyNotFoundException
            => (404, CreateProblemDetails(context, 404, "Resource not found", ex.Message)),

            ArgumentException
            => (400, CreateProblemDetails(context, 400, "Invalid Request", ex.Message)),

            UnauthorizedAccessException
            => (401, CreateProblemDetails(context, 401, "User unauthorized", ex.Message)),
            
            InvalidOperationException
            => (400, CreateProblemDetails(context, 400, "Invalid Request", ex.Message)),

            _ => (500, CreateProblemDetails(context, 500, "An unxpected error occured", "An unxpected error occured while processing request"))
        };


        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problem);

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
