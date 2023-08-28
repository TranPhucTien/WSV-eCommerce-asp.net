using Microsoft.Extensions.Options;
using MongoDB.Driver;
using shopDev.Configurations;
using shopDev.Models;
using shopDev.Utils;

namespace shopDev.Services;

public class ApiKeyService
{
    private readonly IMongoCollection<ApiKeys> _apiKeysCollection;

    public ApiKeyService(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _apiKeysCollection = mongoDb.GetCollection<ApiKeys>(databaseSettings.Value.ApiKeysCollectionName);
    }

    public async Task<ApiKeys> FindById(string id)
    {
        return await _apiKeysCollection.Find(o => o.Id == id).FirstOrDefaultAsync();
    }

    public async Task<ApiKeys> FindByKey(string key)
    {
        // string keyHashed = Hash.GenerateKey();
        // ApiKeys apiKeys = new ApiKeys()
        // {
        //     Status = true,
        //     Key = keyHashed,
        // };
        //
        // await _apiKeysCollection.InsertOneAsync(apiKeys);
        return await _apiKeysCollection.Find(o => o.Key == key).FirstOrDefaultAsync();
    }
}