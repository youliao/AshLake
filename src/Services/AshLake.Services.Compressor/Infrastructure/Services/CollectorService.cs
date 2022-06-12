namespace AshLake.Services.Compressor.Infrastructure.Services;

public interface ICollectorService
{
    Task<Stream?> GetPostFile(string objectKey);
}

public class CollectorService: ICollectorService
{
    private readonly HttpClient _httpClient;

    public CollectorService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Stream?> GetPostFile(string objectKey)
    {
        var response = await _httpClient.GetAsync($"/api/postfiles/{objectKey}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        }

        return await response.Content.ReadAsStreamAsync();
    }
}
