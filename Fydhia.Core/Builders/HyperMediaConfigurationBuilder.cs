﻿using System.Text.Json;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Fydhia.Core.Builders;

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
        if (_typeConfigurationBuilders.SingleOrDefault(type => type.GetTypeToConfigure() == typeof(T)) is TypeConfigurationBuilder<T> typeConfigurationBuilder)
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