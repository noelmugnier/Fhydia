using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fydhia.Core.Configurations;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fydhia.Core.Enrichers;

public interface IHyperMediaObjectEnricher
{
    void Enrich(ExpandoObject value);
}

public class HyperMediaObjectEnricher : IHyperMediaObjectEnricher
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly TypeConfigurationCollection _typeConfigurationConfigurations;

    public HyperMediaObjectEnricher(HyperMediaConfiguration hyperMediaConfiguration, IOptions<JsonOptions> jsonOptions)
    {
        _serializerOptions = jsonOptions.Value.JsonSerializerOptions;
        _typeConfigurationConfigurations = hyperMediaConfiguration.ConfiguredTypes;
    }

    public void Enrich(ExpandoObject resultValues)
    {
        foreach (var resultProperty in resultValues
                     .Where(resultProperty => resultProperty.Value is not null || _serializerOptions.DefaultIgnoreCondition != JsonIgnoreCondition.Never)
                     .Where(resultProperty => resultProperty.Key != "_type")
                     .ToList())
        {
            if (_serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.Always
                || _serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault && resultProperty.Value.IsDefault()
                || _serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull && resultProperty.Value is null)
            {
                resultValues.Remove(resultProperty.Key, out _);
                continue;
            }

            var typeConfiguration = _typeConfigurationConfigurations.GetConfiguration(resultProperty.Value!.GetType());
            if (typeConfiguration is null)
                continue;

            resultValues.Remove(resultProperty.Key, out _);

            var propertyValueProperties = resultProperty.Value.ToExpando();
            Enrich(propertyValueProperties);

            resultValues.TryAdd(resultProperty.Key, propertyValueProperties);
        }
    }
}