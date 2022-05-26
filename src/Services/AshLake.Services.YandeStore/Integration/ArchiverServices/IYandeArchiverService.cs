using Newtonsoft.Json.Linq;

namespace AshLake.Services.YandeStore.Integration.ArchiverServices;

public interface IYandeArchiverService
{
    Task<BsonDocument> GetPostMetadata(int id);
}
