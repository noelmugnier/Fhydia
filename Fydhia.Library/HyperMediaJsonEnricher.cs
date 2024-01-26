using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fydhia.Library;

public class HyperMediaJsonEnricher
{
    private readonly LinkFormatter _linkGenerator;
    private readonly IEnumerable<TypeEnricherConfiguration> _typeEnricherConfigurations;
    private readonly JsonSerializerOptions _serializerOptions;

    public HyperMediaJsonEnricher(HyperMediaConfiguration hyperMediaConfiguration, LinkFormatter linkGenerator)
    {
        _linkGenerator = linkGenerator;
        _typeEnricherConfigurations = hyperMediaConfiguration.ConfiguredTypes;
        _serializerOptions = hyperMediaConfiguration.JsonSerializerOptions;
    }

    public ExpandoObject Enrich(ExpandoObject value, IEnumerable<string> acceptedMediaTypes)
    {
        var formatter = HypermediaTypeFormatterFactory.Create(acceptedMediaTypes, _linkGenerator);

        EnrichValuePropertiesRecursively(value, formatter);

        var typeEnricherConfiguration = GetTypeEnricher(value.GetOriginalType());
        if (typeEnricherConfiguration is null)
            return value;

        return formatter.Format(value, typeEnricherConfiguration);
    }

    private void EnrichValuePropertiesRecursively(ExpandoObject resultValues, HypermediaTypeFormatter formatter)
    {
        foreach (var keyValuePair in resultValues.ToList())
        {
            if(keyValuePair.Value is null && _serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.Never)
                continue;

            if (_serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.Always
                || _serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault && keyValuePair.Value.IsDefault()
                || _serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull && keyValuePair.Value is null)
            {
                resultValues.Remove(keyValuePair.Key, out _);
                continue;
            }

            var propertyEnricher = GetTypeEnricher(keyValuePair.Value.GetType());
            if (propertyEnricher is null)
                continue;

            resultValues.Remove(keyValuePair.Key, out _);

            var propertyValueProperties = keyValuePair.Value.ToExpando();
            EnrichValuePropertiesRecursively(propertyValueProperties, formatter);

            var enrichedProperty = formatter.Format(propertyValueProperties, propertyEnricher);
            resultValues.TryAdd(keyValuePair.Key, enrichedProperty);
        }
    }
    private TypeEnricherConfiguration? GetTypeEnricher(Type? typeToEnrich)
    {
        if(typeToEnrich is null)
            return null;

        return _typeEnricherConfigurations.SingleOrDefault(t => t.TypeToEnrich == typeToEnrich.GetTypeInfo());
    }
}