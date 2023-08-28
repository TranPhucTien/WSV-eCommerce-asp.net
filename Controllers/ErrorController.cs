using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopDev.Utils.Errors;

namespace shopDev.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    public string Error()
    {
        Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        var (statusCode, message) = exception switch
        {
            IServiceException serviceException => ((int) serviceException.StatusCode, serviceException.ErrorMessage),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),
        };

        var rs = new ErrorResponse { Status = statusCode, Title = message, Message = exception?.Message};

        return JsonConvert.SerializeObject(rs);
    }
}