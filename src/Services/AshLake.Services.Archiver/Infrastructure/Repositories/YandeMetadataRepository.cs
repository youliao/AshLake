namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class YandeMetadataRepository<T> : MetadataRepository<T>, IYandeMetadataRepository<T> where T : Metadata
{
    public YandeMetadataRepository(MongoClient mongoClient) : base(mongoClient)
    {
    }

    protected override IMongoDatabase _database => _mongoClient.GetDatabase(BooruSites.Yande);
}
