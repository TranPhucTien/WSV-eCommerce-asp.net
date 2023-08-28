using System.Security.Claims;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using shopDev.Auth;
using shopDev.Configurations;
using shopDev.Models;
using shopDev.Utils;
using shopDev.Utils.Errors;

namespace shopDev.Services;

public class AccessService
{
    private readonly IMongoCollection<Shops> _shopsCollection;
    private readonly KeyTokensService _keyTokensService;
    private readonly ShopsService _shopsService;

    public AccessService(IOptions<DatabaseSettings> databaseSettings, KeyTokensService keyTokensService,
        ShopsService shopsService)
    {
        _keyTokensService = keyTokensService;
        _shopsService = shopsService;

        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _shopsCollection = mongoDb.GetCollection<Shops>(databaseSettings.Value.ShopsCollectionName);
    }

    public async Task<TokenData<Shops>> HandleRefreshToken(string? refreshToken, ClaimsPrincipal shop, KeyTokens? keyToken)
    {
        var userId = shop.Claims.FirstOrDefault(o => o.Type == "userId")!.Value.ToString();
        var email = shop.FindFirstValue(ClaimTypes.Email);

        if (refreshToken is null || userId is null || email is null || keyToken is null)
        {
            throw new BadRequestException("Handle refresh token error");
        }
        
        // 1. If refreshTokensUsed include refreshToken => clear all token by userId
        if (keyToken.RefreshTokenUsed.Contains(refreshToken))
        {
            await _keyTokensService.RemoveByUserId(userId);
            throw new ForBiddenException("Something wrong happen, please re-login");
        }
        
        // 2. Check refreshToken exists in keyToken
        if (string.IsNullOrEmpty(keyToken.RefreshToken))
        {
            throw new NotFoundException("Refresh token not found");
        }

        if (keyToken.RefreshToken != refreshToken)
        {
            throw new AuthFailureException("Shop not registered");
        }
        
        // 3. Check exists shop with email
        var foundShop = await _shopsService.FindByEmail(email);
        if (foundShop is null)
        {
            throw new NotFoundException("Shop not found");
        }
        
        // 4. Create new token and update KeyTokens
        var authUtil = new AuthUtils();
        var tokens = await authUtil.CreateTokenPair(userId, email, keyToken.PublicKey, keyToken.PrivateKey);
        _keyTokensService.UpdateKeyToken(userId, keyToken.PublicKey, keyToken.PrivateKey, tokens.RefreshToken);

        return new TokenData<Shops>
        {
            Data = foundShop,
            TokenPair = tokens
        };
    }

    public async Task<TokenData<Shops>> SignUp(string name, string email, string password)
    {
        // 1. check email exits
        Shops holderShop = await _shopsCollection.Find(o => o.Email == email).FirstOrDefaultAsync();

        if (holderShop != null)
        {
            throw new DuplicateException("Email already used");
        }

        // 2. hash password and save
        string passwordHashed = BCrypt.Net.BCrypt.HashPassword(password);
        Shops shop = new Shops()
        {
            Name = name,
            Email = email,
            Password = passwordHashed
        };
        await _shopsCollection.InsertOneAsync(shop);

        // 3. create privateKey, publicKey
        string privateKey = Hash.GenerateKey();
        string publicKey = Hash.GenerateKey();

        // 4. create keyStore from id, privateKey, publicKey
        var keyStore = _keyTokensService.CreateKeyToken(userid: shop.Id, publicKey, privateKey);
        if (keyStore is null)
        {
            throw new NotFoundException("Not found key store");
        }

        // 5. create tokenPairs from {userId, email}, privateKey, publicKey
        var authUtils = new AuthUtils();
        KeyTokenPair tokens = await authUtils.CreateTokenPair(userId: shop.Id, email, publicKey, privateKey);

        return new TokenData<Shops>
        {
            TokenPair = tokens,
            Data = shop
        };
    }

    public async Task<TokenData<Shops>> Login(string email, string password)
    {
        // 1. Check email
        var foundShop = await _shopsService.FindByEmail(email);
        if (foundShop is null)
        {
            throw new NotFoundException("Not found shop");
        }

        // 2. Match password
        var matchedPassword = BCrypt.Net.BCrypt.Verify(password, foundShop.Password);
        if (!matchedPassword)
        {
            throw new AuthFailureException("Password invalid");
        }

        // 3. Create private key and public key
        var privateKey = Hash.GenerateKey();
        var publicKey = Hash.GenerateKey();

        // 4. Generate token pair from private key, public key and payload
        var authUtil = new AuthUtils();
        var tokens = await authUtil.CreateTokenPair(foundShop.Id, foundShop.Email, publicKey, privateKey);
        _keyTokensService.CreateKeyToken(foundShop.Id, publicKey, privateKey, tokens.RefreshToken);

        // 6. return
        return new TokenData<Shops>
        {
            Data = foundShop,
            TokenPair = tokens
        };
    }

    public async Task<KeyTokens> Logout(KeyTokens? keyToken)
    {
        // 1. Check key token
        if (keyToken is null)
        {
            throw new NotFoundException("Key token not found");
        }

        // 2. Delete key token
        var delKey = await _keyTokensService.RemoveById(keyToken.Id);

        // 3. Return key token
        return delKey;
    }
}