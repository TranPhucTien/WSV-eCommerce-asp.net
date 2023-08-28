using System.Net;

namespace shopDev.Utils.Errors;

public class DuplicateException : Exception, IServiceException
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Conflict;
    public string ErrorMessage { get; set; }

    public DuplicateException(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}