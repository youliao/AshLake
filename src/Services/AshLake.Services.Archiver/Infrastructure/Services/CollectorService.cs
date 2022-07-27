using System.Text.Json.Nodes;

namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface ICollectorService
{
    Task<string> AddDownloadTask(IEnumerable<string> urls, string filename, string md5);

    Task<CollectorService.Aria2GlobalStat?> GetAria2GlobalStat();

    Task<bool> ObjectExists(string objectKey);
}

public class CollectorService : ICollectorService
{
    private readonly HttpClient _httpClient;

    public CollectorService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> AddDownloadTask(IEnumerable<string> urls, string filename, string md5)
    {
        var res = await _httpClient.PostAsJsonAsync("api/aria2/addUri", new { urls, filename, md5 });

        var taskId = await res.Content.ReadAsStringAsync();

        return taskId;
    }

    public async Task<Aria2GlobalStat?> GetAria2GlobalStat()
    {
        return await _httpClient.GetFromJsonAsync<Aria2GlobalStat>("api/aria2/getGlobalStat");
    }


    public async Task<bool> ObjectExists(string objectKey)
    {
        using var res = await _httpClient.GetAsync($"api/s3/objects/{objectKey}");

        if (res.StatusCode is System.Net.HttpStatusCode.NotFound) return false;

        res.EnsureSuccessStatusCode();

        return true;
    }

    public record Aria2GlobalStat(int DownloadSpeed,int NumActive,int NumStopped,int NumStoppedTotal,int NumWaiting, int UploadSpeed);
}
