namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public abstract class PostObjectRepositoty : IPostObjectRepositoty
{
    private readonly DaprClient _daprClient;
    protected abstract string BindingName { get; }

    protected PostObjectRepositoty(DaprClient daprClient)
    {
        _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    }

    public async Task<string> AddOrUpdateAsync(string objectKey, Stream stream)
    {
        var response = await _daprClient.InvokeBindingAsync<string, JsonObject>(BindingName,
                                     DaprBindingOperations.Create,
                                     stream.ToBase64(),
                                     new Dictionary<string, string>() { { "key", objectKey } });

        return response.ToJsonString();
    }

    public async Task DeleteAsync(string objectKey)
    {
        await _daprClient.InvokeBindingAsync(BindingName,
                                     DaprBindingOperations.Delete,
                                     string.Empty,
                                     new Dictionary<string, string>() { { "key", objectKey } });
    }
}
