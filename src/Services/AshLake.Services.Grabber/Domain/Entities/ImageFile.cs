namespace AshLake.Services.Grabber.Domain.Entities;

public class ImageFile
{
    public string PostMD5 { get; private set; }

    public ImageType Type { get; private set; }

    public byte[] Data { get; private set; }

    public string ObjectKey { get => $"{PostMD5}.{Type}".ToLower(); }

    public ImageFile(string postMd5, ImageType type, byte[] data)
    {
        PostMD5 = postMd5?.ToLower() ?? throw new ArgumentNullException(nameof(postMd5));
        Type = type;
        Data = data;
    }
}
