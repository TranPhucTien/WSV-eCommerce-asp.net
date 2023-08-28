using MongoDB.Bson;
using Newtonsoft.Json;
using shopDev.Models;
using shopDev.Models.Enums;
using shopDev.Utils.Errors;

namespace shopDev.Middlewares;

public class CheckPermissionsMiddleware : IMiddleware
{
    private readonly ILogger _logger;
    private readonly EApiKeyPermissions _permission;

    public CheckPermissionsMiddleware(ILogger<CheckPermissionsMiddleware> logger, EApiKeyPermissions permission = EApiKeyPermissions.Default)
    {
        _logger = logger;
        _permission = permission;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Items.ContainsKey("ApiKeyObj") && context.Items["ApiKeyObj"] != null)
        {
            throw new ForBiddenException("Forbidden apikey");
        }
        
        var keyTokens = context.Items["ApiKeyObj"] as ApiKeys;

        if (!(keyTokens is ApiKeys))
        {
            throw new ForBiddenException("Forbidden apikey");
        }
        
        if (!(keyTokens.Permissions.Contains(_permission)))
        {
            throw new ForBiddenException("Forbidden apikey");
        }

        await next(context);
    }
}