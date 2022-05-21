namespace AshLake.Services.Archiver.Infrastructure.Repositories.ObjectStorage;

public abstract class DaprPostObjectRepositoty
{
    private readonly DaprClient _daprClient;
    protected abstract string BindingName { get; }

    protected DaprPostObjectRepositoty(DaprClient daprClient)
    {
        _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    }

    public async Task PutAsync(string objectKey, byte[] data)
    {
        var base64Data = Convert.ToBase64String(data);

        await _daprClient.InvokeBindingAsync(BindingName,
                                     DaprBindingOperations.Create,
                                     base64Data,
                                     new Dictionary<string, string>() { { "key", objectKey } });
    }

    public async Task RemoveAsync(string objectKey)
    {
        await _daprClient.InvokeBindingAsync(BindingName,
                                     DaprBindingOperations.Delete,
                                     string.Empty,
                                     new Dictionary<string, string>() { { "key", objectKey } });
    }
}
