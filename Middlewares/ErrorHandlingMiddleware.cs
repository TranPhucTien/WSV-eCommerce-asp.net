using System.Net;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopDev.Utils.Errors;
using ErrorResponse = shopDev.Utils.Errors.ErrorResponse;

namespace shopDev.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // var code = HttpStatusCode.InternalServerError; // 500 if unexpected
        // var result = JsonConvert.SerializeObject(new { error = exception.Message });
        // context.Response.ContentType = "application/json";
        // context.Response.StatusCode = (int) code;
        // return context.Response.WriteAsync(result);
        
        var (statusCode, message) = exception switch
        {
            IServiceException serviceException => ((int) serviceException.StatusCode, serviceException.ErrorMessage),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var problemDetails = new ErrorResponse
        {
            Status = statusCode,
            Title = message,
            Message = exception.Message
        };

        var json = JsonConvert.SerializeObject(problemDetails);
        await context.Response.WriteAsync(json);
    }
}