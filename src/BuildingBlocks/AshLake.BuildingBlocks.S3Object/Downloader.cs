namespace AshLake.BuildingBlocks.S3Object;

public interface IDownloader
{
    Task<byte[]> DownloadFileTaskAsync(string url);
}
public class Downloader : IDownloader
{
    private readonly HttpClient _httpClient;

    public Downloader(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<byte[]> DownloadFileTaskAsync(string url)
    {
        return await _httpClient.GetByteArrayAsync(url);
    }
}
