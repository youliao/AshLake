using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AshLake.Contracts.Seedwork.Converts;
public class BsonDocumentConverter : JsonConverter<BsonDocument>
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
        writer.WriteRawValue(value.ToJson());

        if (value == null)
        {
            writer.WriteStringValue(String.Empty);
        }
        else
        {
            WriteJson(writer, value);
        }
    }

    private void WriteJson(Utf8JsonWriter writer, BsonDocument bson)
    {
        writer.WriteStartObject();
        var elements = bson.Elements;
        foreach (var item in elements)
        {
            WirteProperty(writer, item.Value, item.Name);
        }
        writer.WriteEndObject();
    }
    private void WirteProperty(Utf8JsonWriter writer, BsonValue bsonValue, string propertyName = null)
    {
        BsonType bsonType = bsonValue.BsonType;
        switch (bsonType)
        {
            case BsonType.EndOfDocument:
                break;
            case BsonType.Int32:
                if (string.IsNullOrEmpty(propertyName))
                {
                    writer.WriteNumberValue(bsonValue.AsInt32);
                }
                else
                {
                    writer.WriteNumber(propertyName, bsonValue.AsInt32);
                }
                break;
            case BsonType.Int64:
                if (string.IsNullOrEmpty(propertyName))
                {
                    writer.WriteNumberValue(bsonValue.AsInt64);
                }
                else
                {
                    writer.WriteNumber(propertyName, bsonValue.AsInt64);
                }
                break;
            case BsonType.Double:
                if (string.IsNullOrEmpty(propertyName))
                {
                    writer.WriteNumberValue(bsonValue.AsDouble);
                }
                else
                {
                    writer.WriteNumber(propertyName, bsonValue.AsDouble);
                }
                break;
            case BsonType.String:
                if (string.IsNullOrEmpty(propertyName))
                {
                    writer.WriteStringValue(bsonValue.AsString);
                }
                else
                {
                    writer.WriteString(propertyName, bsonValue.AsString);
                }
                break;
            case BsonType.Document:
                if (string.IsNullOrEmpty(propertyName))
                {
                    WriteJson(writer, bsonValue.AsBsonDocument);
                }
                else
                {
                    writer.WritePropertyName(propertyName);
                    WriteJson(writer, bsonValue.AsBsonDocument);
                }
                break;
            case BsonType.Array:
                if (string.IsNullOrEmpty(propertyName))
                {
                    var bsonArr = bsonValue.AsBsonArray;
                    writer.WriteStartArray();
                    foreach (var abson in bsonArr)
                    {
                        WirteProperty(writer, abson);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    var bsonArr = bsonValue.AsBsonArray;
                    writer.WritePropertyName(propertyName);
                    writer.WriteStartArray();
                    foreach (var abson in bsonArr)
                    {
                        WirteProperty(writer, abson);
                    }
                    writer.WriteEndArray();
                }
                break;
            case BsonType.Boolean:
                if (string.IsNullOrEmpty(propertyName))
                {
                    writer.WriteBooleanValue(bsonValue.AsBoolean);
                }
                else
                {
                    writer.WriteBoolean(propertyName, bsonValue.AsBoolean);
                }
                break;
            case BsonType.DateTime:
                if (string.IsNullOrEmpty(propertyName))
                {
                    writer.WriteStringValue(bsonValue.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                else
                {
                    writer.WriteString(propertyName, bsonValue.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));
                }
                break;
            case BsonType.Null:
                if (string.IsNullOrEmpty(propertyName))
                {
                    writer.WriteNullValue();
                }
                else
                {
                    writer.WriteNull(propertyName);
                }
                break;
            case BsonType.ObjectId:
                if (string.IsNullOrEmpty(propertyName))
                {
                    writer.WriteStringValue(bsonValue.AsObjectId.ToString());
                }
                else
                {
                    writer.WriteString(propertyName, bsonValue.AsObjectId.ToString());
                }
                break;
            case BsonType.RegularExpression:
                break;
            case BsonType.JavaScript:
                break;
            case BsonType.Symbol:
                break;
            case BsonType.JavaScriptWithScope:
                break;
            case BsonType.Decimal128:
                break;
            case BsonType.MinKey:
                break;
            case BsonType.MaxKey:
                break;
            case BsonType.Timestamp:
                break;
            case BsonType.Binary:
                break;
            case BsonType.Undefined:
                break;
            default:
                break;
        }
    }
}
