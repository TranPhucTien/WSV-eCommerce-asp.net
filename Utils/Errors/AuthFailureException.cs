using System.Net;

namespace shopDev.Utils.Errors;

public class AuthFailureException : Exception, IServiceException
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Unauthorized;
    public string ErrorMessage { get; set; }

    public AuthFailureException(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}