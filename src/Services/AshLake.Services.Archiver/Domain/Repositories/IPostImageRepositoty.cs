using Minio.DataModel;

namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IPostImageRepositoty<T> where T : IStoragble
{
    Task PutAsync(T post);

    Task<bool> ExistsAsync(string objectKey);

    Task RemoveAsync(string objectKey);
}
