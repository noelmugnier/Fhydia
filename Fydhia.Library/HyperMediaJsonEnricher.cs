using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class HyperMediaJsonEnricher
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IEnumerable<TypeEnricherConfiguration> _typeEnricherConfigurations;
    private readonly JsonSerializerOptions _serializerOptions;

    public HyperMediaJsonEnricher(HyperMediaConfiguration hyperMediaConfiguration, LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
        _typeEnricherConfigurations = hyperMediaConfiguration.ConfiguredTypes;
        _serializerOptions = hyperMediaConfiguration.JsonSerializerOptions;
    }

    public ExpandoObject Enrich(HttpContext httpContext, ExpandoObject value)
    {
        var formatter = HypermediaTypeFormatterFactory.Create(httpContext, _linkGenerator);

        EnrichValuePropertiesRecursively(value, formatter);

        var typeEnricherConfiguration = GetTypeEnricher(value.GetOriginalType());
        if (typeEnricherConfiguration is null)
            return value;

        return formatter.Format(value, typeEnricherConfiguration);
    }

    private void EnrichValuePropertiesRecursively(ExpandoObject resultValue, HypermediaTypeFormatter formatter)
    {
        foreach (var keyValuePair in resultValue.ToList())
        {
            if(keyValuePair.Value is null && _serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.Never)
                continue;

            if (_serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.Always
                || _serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault && keyValuePair.Value.IsDefault()
                || _serializerOptions.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull && keyValuePair.Value is null)
            {
                resultValue.Remove(keyValuePair.Key, out _);
                continue;
            }

            var propertyEnricher = GetTypeEnricher(keyValuePair.Value.GetType());
            if (propertyEnricher is null)
                continue;

            resultValue.Remove(keyValuePair.Key, out _);

            var propertyValue = keyValuePair.Value.ToExpando();
            EnrichValuePropertiesRecursively(propertyValue, formatter);

            var enrichedProperty = formatter.Format(propertyValue, propertyEnricher);
            resultValue.TryAdd(keyValuePair.Key, enrichedProperty);
        }
    }
    private TypeEnricherConfiguration? GetTypeEnricher(Type? typeToEnrich)
    {
        if(typeToEnrich is null)
            return null;

        return _typeEnricherConfigurations.SingleOrDefault(t => t.TypeToEnrich == typeToEnrich.GetTypeInfo());
    }
}