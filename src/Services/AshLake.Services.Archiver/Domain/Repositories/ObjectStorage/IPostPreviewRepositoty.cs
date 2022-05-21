using Minio.DataModel;

namespace AshLake.Services.Archiver.Domain.Repositories.ObjectStorage;

public interface IPostPreviewRepositoty
{
    Task PutAsync(string objectKey, byte[] data);

    Task<ObjectStat> StatAsync(string objectKey);

    Task RemoveAsync(string objectKey);
}
