namespace AshLake.Services.Compressor.Domain.Entities;

public class PostPreview : IS3Object
{
    public string PostMD5 { get; private set; } = null!;

    public Stream Data { get; private set; } = null!;

    public string ObjectKey { get => $"{PostMD5}.jpg".ToLower(); }

    public PostPreview(string postMd5, Stream data)
    {
        PostMD5 = postMd5?.ToLower() ?? throw new ArgumentNullException(nameof(postMd5));
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
}
