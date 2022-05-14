using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace AshLake.Services.ArchiveBox.Domain.Entities;

public abstract class Mesadata : IEquatable<Mesadata>
{
    public string Id { get; private set; }

    //[BsonExtraElements]
    public BsonDocument Data { get; private set; }


    protected Mesadata(string id, BsonDocument data)
    {
        Id = id;
        Data = data;
    }

    public bool Equals(Mesadata? other)
    {
        if(other is null || this is null) return false;
        if(this.Id != other.Id) return false;
        if (this.Data.Equals(other.Data) != true) return false;

        return true;
    }
}