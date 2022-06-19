namespace AshLake.Services.Collector.Domain.Entities;

public class PostFile : IS3Object
{
    public string ObjectKey { get; private set; } = null!;

    public byte[] Data { get; private set; }

    public PostFile(string objectKey, byte[] data)
    {
        ObjectKey = objectKey ?? throw new ArgumentNullException(nameof(objectKey));
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
}
