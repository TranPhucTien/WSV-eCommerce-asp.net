using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace shopDev.Models;

public class Shops : BaseEntity
{
    // [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("name")]
    public string Name { get; set; } = "";
    
    [BsonElement("email")]
    public string Email { get; set; } = "";
    
    [BsonElement("password")]
    public string Password { get; set; } = "";
    
    [BsonElement("status")]
    public string Status { get; set; } = "";
    
    [BsonElement("verify")]
    public bool Verify { get; set; } = false;

    [BsonElement("roles")] public List<String> RolesList { get; set; } = new List<string>();
}
