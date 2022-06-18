using System.Security.Cryptography;

namespace AshLake.Services.Collector.Domain.Entities;

public class PostFile : IS3Object
{
    public string PostMD5 { get; private set; } = null!;

    public byte[] Data { get; private set; }

    public string ObjectKey { get; private set; } = null!;

    public PostFile(string postMD5, byte[] data, string objectKey)
    {
        PostMD5 = postMD5 ?? throw new ArgumentNullException(nameof(postMD5));
        Data = data ?? throw new ArgumentNullException(nameof(data));
        ObjectKey = objectKey ?? throw new ArgumentNullException(nameof(objectKey));
    }
}
