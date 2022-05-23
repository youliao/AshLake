namespace AshLake.Services.Grabber.Domain.Repositories;

public interface IYandeSourceSiteRepository
{
    Task<ImageFile> GetFileAsync(int id);
    Task<JsonNode?> GetLatestPostAsync();
    Task<JsonNode?> GetMetadataAsync(int id, bool cachedEnable = true);
    Task<IReadOnlyList<JsonNode>> GetMetadataListAsync(string tags, int limit, int page, bool cachedEnable = true);
    Task<IReadOnlyList<JsonNode>> GetMetadataListAsync(int startId, int limit, int page, bool cachedEnable = true);
    Task<ImageFile> GetPreviewAsync(int id);
}