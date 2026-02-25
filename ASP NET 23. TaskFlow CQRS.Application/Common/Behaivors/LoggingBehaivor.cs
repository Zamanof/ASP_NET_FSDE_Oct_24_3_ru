using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Common.Behaivors;

public class LoggingBehaivor<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string[] SensitiveNames = { "Password", "Secret", "Token", "RefreshToken"};
    private readonly ILogger<LoggingBehaivor<TRequest, TResponse>> _logger;

    public LoggingBehaivor(ILogger<LoggingBehaivor<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        var safeDescription = GetSafeRequestDescrition(request);
        _logger.LogInformation("Handling {RequestName} with data: {RequestData}", requestName, safeDescription);

        var startTime = DateTime.UtcNow;
        try
        {
            var response = await next();
            var elapsed = DateTime.UtcNow - startTime;
            _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds} ms", requestName, elapsed.TotalMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {RequestName}: {ErrorMessage}", requestName, ex.Message);
            throw;
        }
    }

    private static string GetSafeRequestDescrition(TRequest request)
    {
        if (request is null)
            return "(null)";

        var type = request.GetType();
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var parts = new List<string>();
        foreach (var prop in props)
        {
            
            if (SensitiveNames.Any(s => prop.Name.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                parts.Add($"{prop.Name} = ***");
                continue;
            }
            try
            {
                var value = prop.GetValue(request);
                parts.Add($"{prop.Name} = {value}");
            }
            catch 
            {
                parts.Add($"{prop.Name} = ?");
            }
        }
        return string.Join(", ", parts);
    }
}
