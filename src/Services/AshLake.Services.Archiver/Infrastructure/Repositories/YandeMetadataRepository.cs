using Microsoft.Extensions.Options;

namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class YandeMetadataRepository<T> : MetadataRepository<T>, IYandeMetadataRepository<T> where T : Metadata
{
    public YandeMetadataRepository(IOptions<YandeMongoDatabaseSetting> mongoDatabaseSetting) : base(mongoDatabaseSetting)
    {
    }
}
