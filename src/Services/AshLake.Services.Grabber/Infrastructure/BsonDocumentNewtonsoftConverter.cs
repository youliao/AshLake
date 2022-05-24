
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace AshLake.Services.Grabber.Infrastructure;

public class BsonDocumentNewtonsoftConverter : JsonConverter<BsonDocument>
{
    public override BsonDocument ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, [AllowNull] BsonDocument existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        return LiteDB.JsonSerializer.Deserialize(token.ToString()).AsDocument;
    }

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, [AllowNull] BsonDocument value, Newtonsoft.Json.JsonSerializer serializer)
    {
        writer.WriteValue(LiteDB.JsonSerializer.Serialize(value));
    }
}