using System.Text.Json;
using System.Text.Json.Serialization;

namespace AshLake.Services.Archiver.Infrastructure;
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
        return BsonDocument.Parse(json);
    }

    public override void Write(Utf8JsonWriter writer, BsonDocument value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteStringValue(string.Empty);
        else
            writer.WriteRawValue(value.ToJson());
    }
}
