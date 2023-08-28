using MongoDB.Bson.Serialization.Attributes;

namespace shopDev.Models;

public class BaseEntity
{
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("__v")] public int Version { get; set; } = 0;
}