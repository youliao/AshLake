using System.Security.Cryptography;

namespace AshLake.Services.Archiver.Domain.Entities;

public class PostFile : IStoragble
{
    public string PostMD5 { get; private set; } = null!;

    public ImageType Type { get; private set; }

    public byte[] Data { get; private set; } = null!;

    public string ObjectKey { get => ($"{PostMD5}.{Type}").ToLower(); }

    public PostFile(string postMd5, ImageType type, byte[] data)
    {
        PostMD5 = postMd5?.ToLower() ?? throw new ArgumentNullException(nameof(postMd5));
        Type = type;
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public bool IsMatch()
    {
        var md5 = Convert.ToHexString(MD5.HashData(Data))
            .ToLower();

        return md5 == PostMD5;
    }
}
