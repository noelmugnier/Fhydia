using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class HyperMediaEnricher
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IEnumerable<TypeEnricherConfiguration> _typeEnricherConfigurations;

    internal HyperMediaEnricher(LinkGenerator linkGenerator,
        IEnumerable<TypeEnricherConfiguration> typeEnricherConfigurations)
    {
        _linkGenerator = linkGenerator;
        _typeEnricherConfigurations = typeEnricherConfigurations;
    }

    public ExpandoObject Enrich(HttpContext httpContext, ExpandoObject resultValue)
    {
        var formatter = HypermediaTypeFormatterFactory.Create(httpContext, _linkGenerator);

        EnrichTypeProperties(resultValue, formatter);

        var typeEnricher = GetTypeEnricher(resultValue.GetOriginalType());
        if (typeEnricher is null)
            return resultValue;

        return formatter.Format(resultValue, typeEnricher);
    }

    private void EnrichTypeProperties(ExpandoObject resultValue, IHypermediaTypeFormatter formatter)
    {
        foreach (var keyValuePair in resultValue)
        {
            if(keyValuePair.Value is null)
                continue;

            var propertyEnricher = GetTypeEnricher(keyValuePair.Value.GetType());
            if (propertyEnricher is null)
                continue;

            resultValue.Remove(keyValuePair.Key, out _);

            var propertyValue = keyValuePair.Value.ToExpando();
            EnrichTypeProperties(propertyValue, formatter);

            var enrichedProperty = formatter.Format(propertyValue, propertyEnricher);
            resultValue.TryAdd(keyValuePair.Key, enrichedProperty);
        }
    }

    private TypeEnricherConfiguration? GetTypeEnricher(Type getType)
    {
        return _typeEnricherConfigurations.SingleOrDefault(t => t.TypeToEnrich == getType.GetTypeInfo());
    }
}