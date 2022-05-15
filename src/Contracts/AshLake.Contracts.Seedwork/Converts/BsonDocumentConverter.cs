using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AshLake.Contracts.Seedwork.Converts;
public class BsonDocumentConverter : JsonConverter<BsonDocument>
{
    public override BsonDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return BsonDocument.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, BsonDocument value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson());
    }
}
