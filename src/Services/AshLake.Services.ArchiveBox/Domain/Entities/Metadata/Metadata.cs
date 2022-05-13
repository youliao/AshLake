using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace AshLake.Services.ArchiveBox.Domain.Entities;

public abstract class Mesadata
{
    public string Id { get;private set; }

    //[BsonExtraElements]
    public BsonDocument Data { get; private set; }


    protected Mesadata(string id, BsonDocument data)
    {
        Id = id;
        Data = data;
    }
}