using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using shopDev.Models;
using shopDev.Services;
using shopDev.Utils;

namespace shopDev.Controllers;

[ApiController]
[Route("api/shop")]
public class AccessController : Controller
{
    private readonly AccessService _accessService;

    public AccessController(AccessService accessService)
    {
        _accessService = accessService;
    }

    [HttpPost]
    [Route("signup")]
    public async Task<ApiResponse<TokenData<Shops>>> SignUp(Shops shops)
    {
        return new ApiResponse<TokenData<Shops>>
        {
            Status = HttpStatusCode.Created,
            Message = "Created success new shop",
            Data = await _accessService.SignUp(shops.Name, shops.Email, shops.Password)
        };
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<ApiResponse<TokenData<Shops>>> Login(Shops shops)
    {
        return new ApiResponse<TokenData<Shops>>
        {
            Status = HttpStatusCode.OK,
            Message = "Login success",
            Data = await _accessService.Login(shops.Email, shops.Password)
        };
    }
    
    [HttpPost]
    [Route("logout")]
    public async Task<ApiResponse<KeyTokens>> Logout()
    {
        var keyToken = HttpContext.Items[Constants.RequestItem.KeyTokenStore] as KeyTokens;
        return new ApiResponse<KeyTokens>
        {
            Status = HttpStatusCode.OK,
            Message = "Logout success",
            Data = await _accessService.Logout(keyToken)
        };
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<ApiResponse<TokenData<Shops>>> RefreshToken()
    {
        var refreshToken = Request.Headers[Constants.Header.RefreshToken].ToString();
        var keyToken = HttpContext.Items[Constants.RequestItem.KeyTokenStore] as KeyTokens;
        
        var shop = HttpContext.Items[Constants.RequestItem.User] as ClaimsPrincipal;

        return new ApiResponse<TokenData<Shops>>
        {
            Status = HttpStatusCode.OK,
            Message = "Change refresh token access",
            Data = await _accessService.HandleRefreshToken(refreshToken, shop, keyToken)
        };
    }
}