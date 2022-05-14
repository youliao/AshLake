namespace AshLake.Services.ArchiveBox.Domain.Entities;

public class PostMetadata : Mesadata
{
    public PostMetadata(string id, BsonDocument data) : base(id, data)
    {
    }
}
