namespace ASP_NET_21._TaskFlow.BLL.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResponse(
                                        T data, 
                                        string message= "Operation executed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
}
