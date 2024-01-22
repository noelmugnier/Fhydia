using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class HyperMediaEnricher
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IEnumerable<TypeEnricherConfiguration> _typeEnricherConfigurations;

    internal HyperMediaEnricher(LinkGenerator linkGenerator, IEnumerable<TypeEnricherConfiguration> typeEnricherConfigurations)
    {
        _linkGenerator = linkGenerator;
        _typeEnricherConfigurations = typeEnricherConfigurations;
    }

    public ExpandoObject Enrich(HttpContext httpContext, ExpandoObject resultValue)
    {
        var formatter = HypermediaTypeFormatterFactory.Create(httpContext, _linkGenerator);

        //TODO recursive enriching for nested objects

        var typeEnricher = GetTypeEnricher(resultValue.GetOriginalType());
        if (typeEnricher is null)
            return resultValue;

        return formatter.Format(resultValue, typeEnricher);
    }

    private TypeEnricherConfiguration? GetTypeEnricher(Type getType)
    {
        return _typeEnricherConfigurations.SingleOrDefault(t => t.TypeToEnrich == getType.GetTypeInfo());
    }
}