namespace ASP_NET_23._TaskFlow_CQRS.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation executed successfully") =>
        new() { Success = true, Message = message, Data = data };
}
