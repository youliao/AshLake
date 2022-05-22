namespace AshLake.Services.Grabber.Domain.Repositories;

public interface IYandeSourceSiteRepository
{
    Task<ImageFile> GetFileAsync(int id);
    Task<JsonObject?> GetLatestPostAsync();
    Task<JsonObject?> GetMetadataAsync(int id, bool cachedEnable = true);
    Task<IReadOnlyList<JsonObject>> GetMetadataListAsync(string tags, int limit, int page, bool cachedEnable = true);
    Task<IReadOnlyList<JsonObject>> GetMetadataListAsync(int startId, int limit, int page, bool cachedEnable = true);
    Task<ImageFile> GetPreviewAsync(int id);
}