using Minio.DataModel;

namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IPostImageRepositoty<T> where T : IStoragble
{
    Task PutAsync(T post);

    Task<ObjectStat> StatAsync(string objectKey);

    Task RemoveAsync(string objectKey);
}
