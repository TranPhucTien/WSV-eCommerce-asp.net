using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace shopDev.Models;

public class KeyTokens : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("privateKey")]
    public string PrivateKey { get; set; } = "";
    
    [BsonElement("publicKey")]
    public string PublicKey { get; set; } = "";
    
    [BsonElement("refreshToken")]
    public string RefreshToken { get; set; } = "";

    [BsonElement("RefreshTokenUsed")] public List<String> RefreshTokenUsed { get; set; } = new List<string>();
} 