using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Fydhia.Library;

public class HyperMediaConfigurationBuilder
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly MediaTypeCollection _supportedMediaTypes;
    private readonly List<TypeConfigurationBuilder> _typeConfigurationBuilders = new();

    internal HyperMediaConfigurationBuilder(JsonSerializerOptions serializerOptions, MediaTypeCollection supportedMediaTypes)
    {
        _serializerOptions = serializerOptions;
        _supportedMediaTypes = supportedMediaTypes;
    }

    public TypeConfigurationBuilder<T> ConfigureType<T>() where T : class, new()
    {
        TypeConfigurationBuilder<T>? typeConfigurationBuilder =
            _typeConfigurationBuilders.SingleOrDefault(type => type.GetTypeToConfigure() == typeof(T)) as
                TypeConfigurationBuilder<T>;

        if (typeConfigurationBuilder != null)
            return typeConfigurationBuilder;

        typeConfigurationBuilder = new TypeConfigurationBuilder<T>(this);
        _typeConfigurationBuilders.Add(typeConfigurationBuilder);

        return typeConfigurationBuilder;
    }

    public HyperMediaConfiguration Build()
    {
        var typeConfigurations = _typeConfigurationBuilders.Select(builder => builder.Build());
        return new HyperMediaConfiguration(
            new TypeConfigurationCollection(typeConfigurations),
            _serializerOptions,
            _supportedMediaTypes);
    }
}