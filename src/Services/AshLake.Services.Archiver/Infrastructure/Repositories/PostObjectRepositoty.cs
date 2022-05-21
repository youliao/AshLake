namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public abstract class PostObjectRepositoty : IPostObjectRepositoty
{
    private readonly DaprClient _daprClient;
    private readonly string _bindingName;

    private const string _createBindingOperation = "create";
    private const string _deleteBindingOperation = "delete";

    protected PostObjectRepositoty(string bindingName)
    {
        _daprClient = new DaprClientBuilder().Build();
        _bindingName = bindingName ?? throw new ArgumentNullException(nameof(bindingName));
    }

    public async Task AddOrUpdateAsync(string objectKey, string base64Data)
    {
        var result = await _daprClient.InvokeBindingAsync<string,BindingResponse>(_bindingName,
                                     _createBindingOperation,
                                     base64Data,
                                     new Dictionary<string, string>() { { "key", objectKey } });
    }

    public Task DeleteAsync(string key)
    {
        throw new NotImplementedException();
    }
}
