namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface IImgProxyService
{
    Task<bool> Exists(string objectKey);
}

public class ImgProxyService: IImgProxyService
{
    private readonly HttpClient _httpClient;

    public ImgProxyService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<bool> Exists(string objectKey)
    {
        using var res = await _httpClient.GetAsync($"unsafe/raw/plain/{objectKey}");

        if (res.StatusCode is System.Net.HttpStatusCode.NotFound) return false;

        res.EnsureSuccessStatusCode();

        return true;
    }
}
