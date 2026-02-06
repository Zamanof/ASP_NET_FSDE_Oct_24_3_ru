namespace ASP_NET_15._TaskFlow_Refresh_Token.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResponse(
                                    T data, 
                                    string message = "Operation executed successfully")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Message = message,
            Success = true
        };
    }

    public static ApiResponse<T> ErrorResponse(
                                     string message,
                                     T? data = default)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Message = message,
            Success = false
        };
    }
}
