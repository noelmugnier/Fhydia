using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Fydhia.Library;

public class HyperMediaConfigurationBuilder
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly MediaTypeCollection _supportedMediaTypes;
    private readonly List<TypeEnricherBuilder> _typeEnricherBuilders = new();

    internal HyperMediaConfigurationBuilder(JsonSerializerOptions serializerOptions, MediaTypeCollection supportedMediaTypes)
    {
        _serializerOptions = serializerOptions;
        _supportedMediaTypes = supportedMediaTypes;
    }

    public TypeEnricherBuilder<T> ConfigureForType<T>() where T : class, new()
    {
        var typeEnricherBuilder = new TypeEnricherBuilder<T>(this);
        _typeEnricherBuilders.Add(typeEnricherBuilder);
        return typeEnricherBuilder;
    }

    public HyperMediaConfiguration Build()
    {
        return new HyperMediaConfiguration(_typeEnricherBuilders.Select(builder => builder.Build()), _serializerOptions, _supportedMediaTypes);
    }
}