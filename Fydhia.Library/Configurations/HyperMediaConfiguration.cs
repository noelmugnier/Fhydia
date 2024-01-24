using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Fydhia.Library;

public class HyperMediaConfiguration
{
    public IEnumerable<TypeEnricherConfiguration> ConfiguredTypes { get; }
    public JsonSerializerOptions JsonSerializerOptions { get; }
    public MediaTypeCollection SupportedMediaTypes { get; }

    internal HyperMediaConfiguration(IEnumerable<TypeEnricherConfiguration> configuredTypes,
        JsonSerializerOptions jsonSerializerOptions, MediaTypeCollection supportedMediaTypes)
    {
        ConfiguredTypes = configuredTypes;
        JsonSerializerOptions = jsonSerializerOptions;
        SupportedMediaTypes = supportedMediaTypes;
    }
}