using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Fydhia.Library;

public class HyperMediaEnricher
{
    private readonly IEnumerable<TypeEnricher> _typeEnrichers;

    internal HyperMediaEnricher(IEnumerable<TypeEnricher> typeEnrichers)
    {
        _typeEnrichers = typeEnrichers;
    }

    public ExpandoObject Enrich(HttpContext? httpContext, object resultValue)
    {
        var resultType = resultValue.GetType();
        var typeEnricher = GetTypeEnricher(resultType);
        if (typeEnricher is null)
            return resultValue.ToExpando();

        return typeEnricher.Enrich(httpContext!, resultValue.ToExpando());
    }

    private TypeEnricher? GetTypeEnricher(Type getType)
    {
        return _typeEnrichers.SingleOrDefault(t => t.TypeToEnrich == getType.GetTypeInfo());
    }
}