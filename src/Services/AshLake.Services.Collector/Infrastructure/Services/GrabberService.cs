using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace AshLake.Services.Collector.Infrastructure.Services;

public interface IGrabberService<T> where T : IBooru
{
    Task<string?> GetPostFileUrl(int postId);
}

public class GrabberService<T> : IGrabberService<T> where T : IBooru
{
    private readonly HttpClient _httpClient;

    private readonly string _souceSiteName;

    public GrabberService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _souceSiteName = typeof(T).Name.ToLower();
    }

    public async Task<string?> GetPostFileUrl(int postId)
    {
        using var response = await _httpClient.GetAsync($"/api/boorus/{_souceSiteName}/postfileurls/{postId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(); 
    }
}
