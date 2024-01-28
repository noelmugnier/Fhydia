using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Fydhia.Library;

public class HyperMediaConfigurationBuilder
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly MediaTypeCollection _supportedMediaTypes;
    private readonly List<TypeEnricherBuilder> _typeConfigurationBuilders = new();

    internal HyperMediaConfigurationBuilder(JsonSerializerOptions serializerOptions, MediaTypeCollection supportedMediaTypes)
    {
        _serializerOptions = serializerOptions;
        _supportedMediaTypes = supportedMediaTypes;
    }

    public TypeConfigurationBuilder<T> ConfigureType<T>() where T : class, new()
    {
        var typeConfigurationBuilder = new TypeConfigurationBuilder<T>(this);
        _typeConfigurationBuilders.Add(typeConfigurationBuilder);
        return typeConfigurationBuilder;
    }

    public HyperMediaConfiguration Build()
    {
        return new HyperMediaConfiguration(
            new TypeConfigurationCollection(_typeConfigurationBuilders.Select(builder => builder.Build())),
            _serializerOptions,
            _supportedMediaTypes);
    }
}