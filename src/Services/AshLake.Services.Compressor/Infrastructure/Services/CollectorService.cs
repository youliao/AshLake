namespace AshLake.Services.Compressor.Infrastructure.Services;

public interface ICollectorService
{
    Task<string?> GetPostFileUrlAsync(string objectKey);
}

public class CollectorService: ICollectorService
{
    private readonly HttpClient _httpClient;

    public CollectorService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string?> GetPostFileUrlAsync(string objectKey)
    {
        using var response = await _httpClient.GetAsync($"/api/postpresignedurl/{objectKey}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
