namespace AshLake.Services.Compressor.Domain.Entities;

public class PostPreview : IS3Object
{
    public string ObjectKey { get; private set; } = null!;

    public byte[] Data { get; private set; }

    public PostPreview(string objectKey, byte[] data)
    {
        ObjectKey = objectKey ?? throw new ArgumentNullException(nameof(objectKey));
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
}
