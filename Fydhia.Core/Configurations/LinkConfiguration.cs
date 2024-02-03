using System.Dynamic;
using Fydhia.Core.Common;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Configurations;

public abstract class LinkConfiguration
{
    protected readonly IDictionary<string, string> ParameterMappings;

    public string Rel { get; private set; }
    public string? Title { get; init; }
    public string? Name { get; init; }
    public bool Templated { get; init; }

    protected LinkConfiguration(string rel, IDictionary<string, string>? parameterMappings = null)
    {
        Rel = rel.ToSnakeCase();
        ParameterMappings = parameterMappings ?? new Dictionary<string, string>();
    }

    public abstract HyperMediaLink GenerateHyperMediaLink(HttpContext context, LinkGenerator linkGenerator,
        IDictionary<string, object?> returnedObjectProperties);

    protected ExpandoObject BuildActionRouteValues(IDictionary<string, object?> responseObjectProperties)
    {
        var result = new ExpandoObject();
        foreach (var parameterMapping in ParameterMappings)
        {
            if (!responseObjectProperties.TryGetValue(parameterMapping.Value, out var value))
            {
                continue;
            }

            result.TryAdd(parameterMapping.Key, value);
        }

        return result;
    }
}