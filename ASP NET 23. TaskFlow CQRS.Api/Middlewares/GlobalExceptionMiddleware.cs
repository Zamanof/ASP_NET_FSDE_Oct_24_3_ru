using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_23._TaskFlow_CQRS.Api.Middlewares;

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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred while processing request");
        context.Response.ContentType = "application/problem+json";

        var (statusCode, problem) = exception switch
        {
            ValidationException validationException => ((int)HttpStatusCode.BadRequest, CreateValidationProblemDetails(context, validationException, (int)HttpStatusCode.BadRequest)),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, CreateProblemDetails(context, (int)HttpStatusCode.NotFound, "Resource not found", exception.Message)),
            ArgumentException => ((int)HttpStatusCode.BadRequest, CreateProblemDetails(context, (int)HttpStatusCode.BadRequest, "Invalid request", exception.Message)),
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, CreateProblemDetails(context, (int)HttpStatusCode.Unauthorized, "Unauthorized", exception.Message)),
            InvalidOperationException => ((int)HttpStatusCode.BadRequest, CreateProblemDetails(context, (int)HttpStatusCode.BadRequest, "Invalid operation", exception.Message)),
            _ => (500, CreateProblemDetails(context, (int)HttpStatusCode.InternalServerError, "An unexpected error occurred",
                context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true ? exception.Message : "An unexpected error occurred."))
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    private static ProblemDetails CreateProblemDetails(HttpContext context, int statusCode, string title, string? detail = null) =>
        new()
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

    private static ProblemDetails CreateValidationProblemDetails(HttpContext context, ValidationException exception, int statusCode)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
            Title = "One or more validation errors occurred.",
            Status = statusCode,
            Detail = "See the 'errors' property for more details.",
            Instance = context.Request.Path
        };
        problem.Extensions["errors"] = errors;
        return problem;
    }
}
