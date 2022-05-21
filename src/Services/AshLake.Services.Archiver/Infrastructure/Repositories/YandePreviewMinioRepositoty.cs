using Minio;

namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class YandePreviewMinioRepositoty : IYandePreviewRepositoty
{
    private static MinioClient minio = new MinioClient()
                            .WithEndpoint("minio:9000")
                            .WithCredentials("minio", "minio123")
                            .Build();

    public async Task AddOrUpdateAsync(string objectKey, byte[] data)
    {
        using var stream = new MemoryStream(data);

        var args = new PutObjectArgs()
            .WithBucket("yande-preview")
            .WithObject(objectKey)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType("image/jpeg");

        await minio.PutObjectAsync(args);
    }

    public Task DeleteAsync(string objectKey)
    {
        throw new NotImplementedException();
    }
}
