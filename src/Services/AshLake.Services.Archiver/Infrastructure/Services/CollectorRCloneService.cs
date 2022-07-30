using System.Text.Json.Nodes;

namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface ICollectorRCloneService
{
    Task<bool> ObjectExists(string objectKey);
}

public class CollectorRCloneService: ICollectorRCloneService
{
    private readonly HttpClient _httpClient;

    public CollectorRCloneService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<bool> ObjectExists(string objectKey)
    {
        var param = new { fs = "minio:postfile", remote = objectKey };
        var content = new StringContent(param.ToJson());

        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        using var res = await _httpClient.PostAsync("operations/stat", content);
        var result = await res.Content.ReadFromJsonAsync<StatResult>();

        if(result!.Item is null)
            return false;

        return true;
    }

    record StatResult(object Item); 

}
