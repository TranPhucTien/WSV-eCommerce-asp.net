using System.Net;

namespace shopDev.Utils.Errors;

public class NotFoundException : Exception, IServiceException
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage { get; }
    
    public NotFoundException(string errorMessage)
    {
        StatusCode = HttpStatusCode.NotFound;
        ErrorMessage = errorMessage;
    }   
}