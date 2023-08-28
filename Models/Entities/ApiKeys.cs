using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using shopDev.Models.Enums;

namespace shopDev.Models;

public class ApiKeys : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("key")] public string Key { get; set; } = "";

    [BsonElement("status")] public bool Status { get; set; } = true;

    [BsonElement("permissions")] public List<EApiKeyPermissions> Permissions { get; set; } = new List<EApiKeyPermissions>
        { EApiKeyPermissions.Default };

}