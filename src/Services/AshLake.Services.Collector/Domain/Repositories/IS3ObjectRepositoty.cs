using Minio.DataModel;

namespace AshLake.Services.Collector.Domain.Repositories;

public interface IS3ObjectRepositoty<T> where T : IS3Object
{
    Task PutAsync(T post);

    Task<bool> ExistsAsync(string objectKey);

    Task<Stream?> GetStreamAsync(string objectKey);

    Task RemoveAsync(string objectKey);
}
