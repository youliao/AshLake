using Minio;
using Minio.DataModel;

namespace AshLake.BuildingBlocks.S3Object;

public interface IS3ObjectRepositoty<T> where T : IS3Object
{
    Task PutAsync(T post);

    Task<bool> ExistsAsync(string objectKey);
    Task<ObjectStat?> StatObjectAsync(string objectKey);

    Task<Stream?> GetDataAsync(string objectKey);

    Task<string?> GetPresignedUrlAsync(string objectKey);

    Task RemoveAsync(string objectKey);

}

public class S3ObjectRepositoty<T> : IS3ObjectRepositoty<T> where T : IS3Object
{
    private readonly MinioClient _minioClient;
    private readonly string _bucketName = typeof(T).Name.ToLower();

    public S3ObjectRepositoty(MinioClient minioClient)
    {
        _minioClient = minioClient ?? throw new ArgumentNullException(nameof(minioClient));

        CreateBucketAsync(_bucketName).Wait();
    }

    public async Task PutAsync(T s3Object)
    {
        var args = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(s3Object.ObjectKey);

        using var stream = new MemoryStream();

        if (s3Object.Data.CanSeek)
        {
            s3Object.Data.Position = 0;
            args = args
                .WithStreamData(s3Object.Data)
                .WithObjectSize(s3Object.Data.Length);
        }
        else
        {
            s3Object.Data.CopyTo(stream);
            stream.Position = 0;
            args = args
                .WithStreamData(stream)
                .WithObjectSize(stream.Length);
        }

        await _minioClient.PutObjectAsync(args);
    }

    public async Task<bool> ExistsAsync(string objectKey)
    {
        var args = new StatObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey);

        try
        {
            var stat = await _minioClient.StatObjectAsync(args);
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

    public async Task<ObjectStat?> StatObjectAsync(string objectKey)
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
            return null;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Stream?> GetDataAsync(string objectKey)
    {
        var data = new MemoryStream();
        var args = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(data);
                stream.Dispose();
            });

        try
        {
            await _minioClient.GetObjectAsync(args);
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            return null;
        }
        catch
        {
            throw;
        }

        data.Position = 0;

        return data;
    }

    public async Task<string?> GetPresignedUrlAsync(string objectKey)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithExpiry(600);

        try
        {
            return await _minioClient.PresignedGetObjectAsync(args);
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            return null;
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
    }

    private async Task<bool> BucketExistsAsync(string bucketName)
    {
        var args = new BucketExistsArgs()
            .WithBucket(bucketName);

        return await _minioClient.BucketExistsAsync(args);
    }
}