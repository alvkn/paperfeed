using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaperFeed.Infrastructure;

public class SingleOrArrayConverter<T> : JsonConverter<List<T>>
{
    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new List<T>();

        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
            {
                var item = JsonSerializer.Deserialize<T>(ref reader, options);
                result.Add(item!);
                break;
            }
            case JsonTokenType.StartArray:
                result = JsonSerializer.Deserialize<List<T>>(ref reader, options)!;
                break;
            default:
                throw new JsonException($"Unexpected token {reader.TokenType}");
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        if (value.Count == 1)
        {
            JsonSerializer.Serialize(writer, value[0], options);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}