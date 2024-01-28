using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public abstract class LinkConfiguration
{
    protected readonly IDictionary<string, string> ParameterMappings;
    protected readonly HttpVerb Verb;
    protected readonly IEnumerable<ParsedParameter> ParsedParameters;
    protected readonly ReturnedType ReturnedType;
    protected readonly Resource Group;

    public string Rel { get; private set; }
    public string? Title { get; init; }
    public string? Name { get; init; }
    public bool? Templated { get; init; }

    protected LinkConfiguration(string rel, HttpVerb verb, ReturnedType returnedType, Resource group,
        IEnumerable<ParsedParameter> parsedParameters, IDictionary<string, string>? parameterMappings = null)
    {
        Verb = verb;
        Group = group;
        ParsedParameters = parsedParameters;
        Rel = rel;
        ReturnedType = returnedType;
        ParameterMappings = parameterMappings ?? new Dictionary<string, string>();
    }

    public abstract HyperMediaLink GenerateHyperMediaLink(HttpContext context, LinkGenerator linkGenerator,
        IDictionary<string, object?> returnedObjectProperties);

    public void ValidateParameterMappings()
    {
        if (ParsedParameters.Count(p =>
                p.BindingSource == BindingSource.Path
                || p.BindingSource == BindingSource.Query
                || p.BindingSource == BindingSource.Header) != ParameterMappings.Count)
        {
            throw new InvalidOperationException(
                $"Parameter mappings for link configuration {Rel} are invalid. Parameter mappings must be provided for all parameters in the method signature that are bound to the path, query or header.");
        }
    }

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