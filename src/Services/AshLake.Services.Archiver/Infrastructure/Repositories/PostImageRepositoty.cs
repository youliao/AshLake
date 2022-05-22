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

        _bucketName = typeof(T).Name switch
        {
            nameof(PostFile) => "post-file",
            nameof(PostPreview) => "post-preview",
            _ => throw new NotSupportedException(typeof(T).Name)
        };

        CreateBucketAsync(_bucketName).Wait();
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

    public async Task<bool> ExistsAsync(string objectKey)
    {
        var args = new StatObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey);

        try
        {
            await _minioClient.StatObjectAsync(args);
            return true;
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            return false;
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

    private async Task CreateBucketAsync(string bucketName)
    {
        var isExists = await BucketExistsAsync(bucketName);
        if (isExists) return;

        var args = new MakeBucketArgs()
            .WithBucket(bucketName);
        await _minioClient.MakeBucketAsync(args);

        await SetBucketPolicyAsync(bucketName);
    }

    private async Task<bool> BucketExistsAsync(string bucketName)
    {
        var args = new BucketExistsArgs()
            .WithBucket(bucketName);

        return await _minioClient.BucketExistsAsync(args);
    }

    private async Task SetBucketPolicyAsync(string bucketName)
    {
        var policyJson = $"{{\"Version\":\"2012-10-17\",\"Statement\":[{{\"Effect\":\"Allow\",\"Principal\":{{\"AWS\":[\"*\"]}},\"Action\":[\"s3:GetObject\"],\"Resource\":[\"arn:aws:s3:::{bucketName}/*\"]}}]}}";
        var args = new SetPolicyArgs()
            .WithBucket(bucketName)
            .WithPolicy(policyJson);

        await _minioClient.SetPolicyAsync(args);
    }
}
