using Newtonsoft.Json.Linq;

namespace AshLake.Services.Grabber.Domain.Repositories;

public interface IYandeSourceSiteRepository
{
    Task<ImageFile> GetFileAsync(int id);
    Task<JToken> GetLatestPostAsync();
    Task<JToken?> GetMetadataAsync(int id, bool cachedEnable = true);
    Task<IEnumerable<JToken>> GetMetadataListAsync(string tags, int limit, int page, bool cachedEnable = true);
    Task<IEnumerable<JToken>> GetMetadataListAsync(int startId, int limit, int page, bool cachedEnable = true);
    Task<ImageFile> GetPreviewAsync(int id);
}