using AshLake.Services.Archiver.Domain.Repositories.ObjectStorage;
using AshLake.Services.Archiver.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;

namespace AshLake.Services.Archiver.Infrastructure.Repositories.ObjectStorage;

public class PostPreviewRepositoty : IPostPreviewRepositoty
{
    private readonly MinioClient _minioClient;
    private const string Bucket = "post-preview";

    public PostPreviewRepositoty(IOptions<PostPreviewStorageSetting> setting)
    {
        _minioClient = new MinioClient()
                            .WithEndpoint(setting.Value.Endpoint)
                            .WithCredentials(setting.Value.AccessKey, setting.Value.SecretKey)
                            .Build();
    }

    public async Task PutAsync(string objectKey, byte[] data)
    {
        using var stream = new MemoryStream(data);

        var args = new PutObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectKey)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length);

        await _minioClient.PutObjectAsync(args);
    }

    public async Task<ObjectStat> StatAsync(string objectKey)
    {
        var args = new StatObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectKey);

        try
        {
            return await _minioClient.StatObjectAsync(args);
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            return null!;
        }
        catch
        {
            throw;
        }

    }

    public async Task RemoveAsync(string objectKey)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(args);
    }
}
