namespace AshLake.Services.Archiver.Domain.Repositories;

public interface IYandeMetadataRepository<T> : IMetadataRepository<T> where T : Metadata
{
}
