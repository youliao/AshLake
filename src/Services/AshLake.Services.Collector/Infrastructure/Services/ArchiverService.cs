namespace AshLake.Services.Collector.Infrastructure.Services;

public interface IArchiverService<T> where T : IBooru
{
    Task<string?> GetPostObjectKey(int postId);
}

public class ArchiverService<T> : IArchiverService<T> where T : IBooru
{
    private readonly HttpClient _httpClient;

    private readonly string _booruName;

    public ArchiverService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _booruName = typeof(T).Name.ToLower();
    }

    public async Task<string?> GetPostObjectKey(int postId)
    {
        using var response = await _httpClient.GetAsync($"/api/boorus/{_booruName}/postobjectkeys/{postId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        var objectKey = await response.Content.ReadAsStringAsync();
        return objectKey;
    }
}
