using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using shopDev.Models;
using shopDev.Utils.Errors;

namespace shopDev.Auth;

public class AuthUtils
{
    public Task<KeyTokenPair> CreateTokenPair(string userId, string email, string publicKey, string privateKey)
    {
        // payload
        var claims = new[]
        {
            new Claim("userId", userId),
            new Claim("email", email)
        };

        // generate token
        var accessToken = GenerateJwtToken(payload: claims, publicKey, expires: 2);
        var refreshToken = GenerateJwtToken(payload: claims, privateKey, expires: 7);
        
        // generate jwt
        var handler = new JwtSecurityTokenHandler();
        var accessTokenString = handler.WriteToken(accessToken);
        var refreshTokenString = handler.WriteToken(refreshToken);

        return Task.FromResult(new KeyTokenPair
        {
            AccessToken = accessTokenString,
            RefreshToken = refreshTokenString
        });
    }

    private JwtSecurityToken GenerateJwtToken(Claim[] payload, string secretKey, int expires)
    {
        return new JwtSecurityToken(
            claims: payload,
            expires: DateTime.Now.AddDays(expires),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256Signature
            ));
    }
    
    public ClaimsPrincipal JwtDecoder(string jwtToken, string secretKey)
    {
        var handler = new JwtSecurityTokenHandler();
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = true
        };
        
        try
        {
            var claimsPrincipal = handler.ValidateToken(jwtToken, tokenValidationParameters, out _);
            
            return claimsPrincipal;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying access token: {ex.Message}");
            throw new BadRequestException("Decode failure");
        }
    }
}