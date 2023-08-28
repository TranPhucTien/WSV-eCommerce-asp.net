using Microsoft.Extensions.Options;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using shopDev.Configurations;
using shopDev.Models;

namespace shopDev.Services;

public class ShopsService
{
    private readonly IMongoCollection<Shops> _shopsCollection;

    public ShopsService(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _shopsCollection = mongoDb.GetCollection<Shops>(databaseSettings.Value.ShopsCollectionName);
    }

    public async Task<Shops> FindByEmail(string email) => await _shopsCollection.Find(o => o.Email == email).FirstOrDefaultAsync();
    public async Task<List<Shops>> GetAsync() => await _shopsCollection.Find(_ => true).ToListAsync();
    public async Task<Shops> GetAsync(string id) => await _shopsCollection.Find(o => o.Id == id).FirstOrDefaultAsync();
    
    public async Task CreateAsync(Shops shops) => await _shopsCollection.InsertOneAsync(shops);
    
    public async Task UpdateAsync(Shops shops) => await _shopsCollection.ReplaceOneAsync(o => o.Id == shops.Id, shops);
    
    public async Task RemoveAsync(string id) => await _shopsCollection.DeleteOneAsync(o => o.Id == id);
}
