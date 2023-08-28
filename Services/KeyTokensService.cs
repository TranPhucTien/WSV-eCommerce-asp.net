using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using shopDev.Configurations;
using shopDev.Models;
using shopDev.Utils.Errors;

namespace shopDev.Services;

public class KeyTokensService
{
    private readonly IMongoCollection<KeyTokens> _keyTokensCollection;

    public KeyTokensService(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _keyTokensCollection = mongoDb.GetCollection<KeyTokens>(databaseSettings.Value.KeyTokensCollectionName);
    }

    public string CreateKeyToken(string userid, string publicKey, string privateKey,
        string refreshToken = "")
    {
        UpdateKeyToken(userid, publicKey, privateKey, refreshToken);
        
        // 2. return publicKey
        return publicKey;
    }

    public async Task<KeyTokens> RemoveById(string id)
    {
        return await _keyTokensCollection.FindOneAndDeleteAsync(o => o.Id == id);
    }
    
    public async Task<KeyTokens> RemoveByUserId(string userId)
    {
        return await _keyTokensCollection.FindOneAndDeleteAsync(o => o.UserId == userId);
    }

    public async Task<KeyTokens> FindByUserId(string userId)
    {
        return await _keyTokensCollection.Find(o => o.UserId == userId).FirstOrDefaultAsync();
    }

    public async void UpdateKeyToken(string userid, string publicKey, string privateKey, string refreshToken)
    {
        try
        {
            var keyToken = await FindByUserId(userid);

            if (keyToken is null)
            {
                var newToken = new KeyTokens()
                {
                    UserId = userid,
                    PublicKey = publicKey,
                    PrivateKey = privateKey,
                    RefreshToken = refreshToken
                };
                await _keyTokensCollection.InsertOneAsync(newToken);
            }
            else
            {
                var newToken = keyToken;
                newToken.PublicKey = publicKey;
                newToken.PrivateKey = privateKey;
                newToken.RefreshTokenUsed.Add(newToken.RefreshToken);
                newToken.RefreshToken = refreshToken;
                await _keyTokensCollection.ReplaceOneAsync(o => o.UserId == keyToken.UserId, newToken);
            }
        }
        catch (Exception e)
        {
            throw new BadRequestException("Update key token error");
        }
    }
}