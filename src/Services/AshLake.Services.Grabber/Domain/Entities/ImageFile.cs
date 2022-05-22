namespace AshLake.Services.Grabber.Domain.Entities;

public class ImageFile
{
    public string PostMD5 { get; private set; } = null!;

    public ImageType Type { get; private set; }

    public Stream Data { get; private set; } = null!;

    public string ObjectKey { get => $"{PostMD5}.{Type}".ToLower(); }

    public ImageFile(string postMd5, ImageType type, Stream data)
    {
        PostMD5 = postMd5?.ToLower() ?? throw new ArgumentNullException(nameof(postMd5));
        Type = type;
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
}
