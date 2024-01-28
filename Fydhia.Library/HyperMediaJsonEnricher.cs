using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fydhia.Library;

public interface IHyperMediaJsonEnricher
{
    void Enrich(ExpandoObject value);
}

public class HyperMediaJsonEnricher : IHyperMediaJsonEnricher
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly TypeConfigurationCollection _typeConfigurationConfigurations;

    public HyperMediaJsonEnricher(HyperMediaConfiguration hyperMediaConfiguration)
    {
        _serializerOptions = hyperMediaConfiguration.JsonSerializerOptions;
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