using AshLake.Contracts.Seedwork;

namespace AshLake.Services.ArchiveBox.Infrastructure.Repositories;

public class YandeMetadataRepository<T> : MetadataRepository<T>, IYandeMetadataRepository<T> where T : Mesadata
{
    public YandeMetadataRepository(MongoClient mongoClient) : base(mongoClient)
    {
    }

    protected override IMongoDatabase _database => _mongoClient.GetDatabase(BooruSites.Yande);
}
