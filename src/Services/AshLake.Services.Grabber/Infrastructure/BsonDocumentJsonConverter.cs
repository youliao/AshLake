using System.Text.Json;
using System.Text.Json.Serialization;

namespace AshLake.Services.Grabber.Infrastructure;

public class BsonDocumentJsonConverter : JsonConverter<BsonDocument>
{
    public override BsonDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        using var stream = new MemoryStream();
        using Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        jsonDoc.WriteTo(writer);
        writer.Flush();
        string json = System.Text.Encoding.UTF8.GetString(stream.ToArray());

        return LiteDB.JsonSerializer.Deserialize(json).AsDocument;
    }

    public override void Write(Utf8JsonWriter writer, BsonDocument value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(LiteDB.JsonSerializer.Serialize(value));
    }
}