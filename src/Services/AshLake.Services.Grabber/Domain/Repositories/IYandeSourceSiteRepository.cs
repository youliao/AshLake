namespace AshLake.Services.Grabber.Domain.Repositories;

public interface IYandeSourceSiteRepository
{
    Task<ImageFile> GetFileAsync(int id);
    Task<BsonDocument> GetLatestPostAsync();
    Task<BsonDocument?> GetMetadataAsync(int id, bool cachedEnable = true);
    Task<IEnumerable<BsonDocument>> GetMetadataListAsync(string tags, int limit, int page, bool cachedEnable = true);
    Task<IEnumerable<BsonDocument>> GetMetadataListAsync(int startId, int limit, int page, bool cachedEnable = true);
    Task<ImageFile> GetPreviewAsync(int id);
}