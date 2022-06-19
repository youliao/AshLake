namespace AshLake.BuildingBlocks.S3Object;

public interface IDownloader
{
    Task<Stream> DownloadFileAsync(string url);
}
public class Downloader : IDownloader
{
    private readonly HttpClient _httpClient;

    public Downloader(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Stream> DownloadFileAsync(string url)
    {
        return await _httpClient.GetStreamAsync(url);
    }
}
