using System.Net;

namespace shopDev.Utils.Errors;

public class BadRequestException : Exception, IServiceException
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;
    public string ErrorMessage { get; set; }

    public BadRequestException(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}