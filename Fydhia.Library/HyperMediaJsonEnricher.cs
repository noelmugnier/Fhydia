using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Fydhia.Library;

public interface IHyperMediaJsonEnricher
{
    ExpandoObject Enrich(ExpandoObject value, HttpContext httpContext);
}

public class HyperMediaJsonEnricher : IHyperMediaJsonEnricher
{
    private readonly IProvideHyperMediaTypeFormatter _hyperMediaTypeFormatterProvider;
    private readonly IEnumerable<TypeEnricherConfiguration> _typeEnricherConfigurations;
    private readonly JsonSerializerOptions _serializerOptions;

    public HyperMediaJsonEnricher(HyperMediaConfiguration hyperMediaConfiguration, IProvideHyperMediaTypeFormatter hyperMediaTypeFormatterProvider)
    {
        _hyperMediaTypeFormatterProvider = hyperMediaTypeFormatterProvider;
        _typeEnricherConfigurations = hyperMediaConfiguration.ConfiguredTypes;
        _serializerOptions = hyperMediaConfiguration.JsonSerializerOptions;
    }

    public ExpandoObject Enrich(ExpandoObject value, HttpContext httpContext)
    {
        var formatter = _hyperMediaTypeFormatterProvider.GetFormatter(httpContext.Request.GetAcceptedMediaTypes());

        EnrichValuePropertiesRecursively(value, formatter, httpContext);

        var typeEnricherConfiguration = GetTypeEnricher(value.GetOriginalType());
        if (typeEnricherConfiguration is null)
            return value;

        return formatter.Format(value, typeEnricherConfiguration, httpContext);
    }

    private void EnrichValuePropertiesRecursively(ExpandoObject resultValues, HypermediaTypeFormatter formatter, HttpContext httpContext)
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
            EnrichValuePropertiesRecursively(propertyValueProperties, formatter, httpContext);

            var enrichedProperty = formatter.Format(propertyValueProperties, propertyEnricher, httpContext);
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