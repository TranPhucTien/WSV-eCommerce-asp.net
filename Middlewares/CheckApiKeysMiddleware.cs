using shopDev.Services;
using shopDev.Utils;
using shopDev.Utils.Errors;

namespace shopDev.Middlewares;

public class CheckApiKeysMiddleware : IMiddleware
{
    private readonly ILogger _logger;
    private readonly ApiKeyService _apiKeyService;

    public CheckApiKeysMiddleware(ILogger<CheckApiKeysMiddleware> logger, ApiKeyService apiKeyService)
    {
        _logger = logger;
        _apiKeyService = apiKeyService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.ContainsKey(Constants.Header.ApiKey))
        {
            throw new NotFoundException("Not found key store");
        }

        string apiKey = context.Request.Headers[Constants.Header.ApiKey].ToString();

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new NotFoundException("Not found key store");
        }
        
        var objKey = await _apiKeyService.FindByKey(apiKey);
        
        if (objKey is null)
        {
            throw new NotFoundException("Not found key store");
        }
        
        context.Items[Constants.RequestItem.ApiKeyObj] = objKey;
        await next(context);
    }
}