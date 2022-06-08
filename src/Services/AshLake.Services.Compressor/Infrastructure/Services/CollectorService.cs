using AshLake.Services.Compressor.Application.Services;

namespace AshLake.Services.Compressor.Infrastructure.Services;

public class CollectorService: ICollectorService
{
    private readonly HttpClient _httpClient;

    public CollectorService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Stream> GetPostFile(string objectKey)
    {
        return await _httpClient.GetStreamAsync($"/postfile/{objectKey}");
    }
}
