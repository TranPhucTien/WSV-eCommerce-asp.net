namespace shopDev.Utils.Errors;

public class ErrorResponse
{
    public string Type { get; set; } = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
    public string Title { get; set; } = "";
    public int Status { get; set; }
    public string? Message { get; set; } = "";
}