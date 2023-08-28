using System.Net;

namespace shopDev.Models;

public class ApiResponse<T>
{
    public ApiResponse(HttpStatusCode status, string message, T data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    public ApiResponse()
    {
    }

    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}