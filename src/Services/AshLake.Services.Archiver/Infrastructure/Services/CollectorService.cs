using EasyCaching.Core;

namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface ICollectorService
{
    Task<string?> AddDownloadTask(IEnumerable<string> urls, string filename, string md5);
}

public class CollectorService : ICollectorService
{
    private readonly HttpClient _httpClient;

    public CollectorService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string?> AddDownloadTask(IEnumerable<string> urls, string filename, string md5)
    {
        var res = await _httpClient.PostAsJsonAsync("api/aria2/addUri", new { urls, filename, md5 });

        var taskId = await res.Content.ReadAsStringAsync();

        return taskId;
    }
}
