using System.Net;

namespace shopDev.Utils.Errors;

public class ForBiddenException : Exception, IServiceException
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Forbidden;
    public string ErrorMessage { get; set; }
    
    public ForBiddenException(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}
