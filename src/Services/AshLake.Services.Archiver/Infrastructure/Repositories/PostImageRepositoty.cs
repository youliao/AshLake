using Minio;
using Minio.DataModel;

namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class PostImageRepositoty<T> : IPostImageRepositoty<T> where T : IStoragble
{
    private readonly MinioClient _minioClient;
    private readonly string _bucketName;

    public PostImageRepositoty(MinioClient minioClient)
    {
        _minioClient = minioClient ?? throw new ArgumentNullException(nameof(minioClient));

        if (typeof(T) == typeof(PostFile)) _bucketName = "post-file";
        if (typeof(T) == typeof(PostPreview)) _bucketName = "post-preview";

        Guard.Against.Null(_bucketName);
    }

    public async Task PutAsync(T post)
    {
        using var stream = new MemoryStream(post.Data);

        var args = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(post.ObjectKey)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length);

        await _minioClient.PutObjectAsync(args);
    }

    public async Task<ObjectStat> StatAsync(string objectKey)
    {
        var args = new StatObjectArgs()
            .WithBucket(_bucketName)
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
            .WithBucket(_bucketName)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(args);
    }
}
