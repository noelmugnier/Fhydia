using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Fydhia.Core.Common;

public class HyperMediaHalLink
{
    public HyperMediaHalLink(string href, bool? templated = false, string? title = null, string? name = null, IHeaderDictionary? headers = null)
    {
        Href = href;
        Templated = templated;
        Title = title;
        Name = name;
        Headers = headers;
    }

    public string Href { get; init; }
    public bool? Templated { get; init; }
    public string? Title { get; init; }
    public string? Name { get; init; }

    [JsonConverter(typeof(CaseInsensitiveDictionaryConverter))]
    public IHeaderDictionary? Headers { get; init; }
}

public sealed class CaseInsensitiveDictionaryConverter : JsonConverter<IHeaderDictionary>
{
    public override void Write(Utf8JsonWriter writer, IHeaderDictionary value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var (key, headerValues) in value)
        {
            writer.WritePropertyName(key);
            if(headerValues.Count == 1)
            {
                writer.WriteStringValue(headerValues[0]);
            }
            else
            {
                writer.WriteStartArray();
                foreach (var headerValue in headerValues)
                {
                    writer.WriteStringValue(headerValue);
                }
                writer.WriteEndArray();
            }
        }
        writer.WriteEndObject();
    }

    public override IHeaderDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}