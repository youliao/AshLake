namespace AshLake.Services.Collector.Domain.Entities;

public class PostPreview : IS3Object
{
    public string PostMD5 { get; private set; } = null!;

    public byte[] Data { get; private set; } = null!;

    public const ImageType Type = ImageType.JPG;

    public string ObjectKey { get => $"{PostMD5}.{Type}".ToLower(); }

    public PostPreview(string postMd5, byte[] data)
    {
        PostMD5 = postMd5?.ToLower() ?? throw new ArgumentNullException(nameof(postMd5));
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
}
