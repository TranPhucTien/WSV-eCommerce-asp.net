using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using shopDev.Auth;
using shopDev.Models;
using shopDev.Services;
using shopDev.Utils;
using shopDev.Utils.Errors;

namespace shopDev.Middlewares;

public class AuthenticationMiddleware : IMiddleware
{
    private readonly ILogger _logger;
    private readonly KeyTokensService _keyTokensService;

    public AuthenticationMiddleware(ILogger<AuthenticationMiddleware> logger, KeyTokensService keyTokensService)
    {
        _logger = logger;
        _keyTokensService = keyTokensService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 1. Check userId
        var userId = context.Request.Headers[Constants.Header.ClientId].ToString();
        if (userId is null)
        {
            throw new NotFoundException("Not found user id");
        }
        
        // 2. Get access token
        var keyTokens = await _keyTokensService.FindByUserId(userId);
        if (keyTokens is null)
        {
            throw new NotFoundException("Not found key token");
        }
        
        // Check refresh token
        bool hasCalledNext = false;
        var refreshToken = context.Request.Headers[Constants.Header.RefreshToken].ToString();
        var authUtil = new AuthUtils();
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var userDecode = authUtil.JwtDecoder(refreshToken, keyTokens.PrivateKey);
            
            string useIdDecoded = userDecode.Claims.FirstOrDefault(o => o.Type == "userId")!.Value.ToString();
            if (userId != useIdDecoded)
            {
                throw new AuthFailureException("Decode failure");
            }
            
            context.Items[Constants.RequestItem.KeyTokenStore] = keyTokens;
            context.Items[Constants.RequestItem.User] = userDecode;
            context.Items[Constants.Header.RefreshToken] = refreshToken;

            await next(context);
        }

        if (hasCalledNext)
        {
            var accessToken = context.Request.Headers[Constants.Header.Authorization].ToString();
            if (accessToken is null)
            {
                throw new AuthFailureException("Not authorization");
            }

            // 3. Verify token
            var userDecode = authUtil.JwtDecoder(accessToken, keyTokens.PublicKey);
        
            // 4. Check user with token
            string useIdDecoded = userDecode.Claims.FirstOrDefault(o => o.Type == "userId")!.Value.ToString();
            if (userId != useIdDecoded)
            {
                throw new AuthFailureException("Decode failure");
            }
        
            // 5. Send keystore with context
            context.Items[Constants.RequestItem.KeyTokenStore] = keyTokens;
            context.Items[Constants.RequestItem.User] = userDecode;
        
            // 6. Ok all
            await next(context);
        }
    }
}