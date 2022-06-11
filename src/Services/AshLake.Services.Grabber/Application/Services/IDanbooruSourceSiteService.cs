using Newtonsoft.Json.Linq;

namespace AshLake.Services.Grabber.Domain.Services;

public interface IDanbooruSourceSiteService
{
    Task<ImageFile> GetFileAsync(int id);
    Task<ImageFile> GetPreviewAsync(int id);
    Task<JToken> GetLatestPostMetadataAsync();
    Task<JToken?> GetPostMetadataAsync(int id);
    Task<IEnumerable<JToken>> GetPostMetadataListAsync(string tags, int limit, int page);
    Task<IEnumerable<JToken>> GetPostMetadataListAsync(int startId, int limit, int page);
    Task<IEnumerable<JToken>> GetTagMetadataListAsync(int? type, int limit, int page);
}