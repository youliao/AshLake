namespace AshLake.Services.Archiver.Integration.GrabberServices;

public interface IGrabberService
{
    Task<List<BsonDocument>> GetPostMetadataList(int startId, int limit);
}