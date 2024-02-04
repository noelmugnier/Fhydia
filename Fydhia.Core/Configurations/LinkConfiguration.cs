using System.Dynamic;
using System.Reflection;
using Fydhia.Core.Common;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Configurations;

public abstract class LinkConfiguration
{
    protected readonly IDictionary<string, string> ParameterMappings;

    protected readonly LinkOptions LinkOptions = new()
    {
        LowercaseUrls = true,
        LowercaseQueryStrings = true,
        AppendTrailingSlash = false
    };

    public string Rel { get; }
    public string? Title { get; internal init; }
    public string? Name { get; internal init; }
    public bool? Templated { get; internal init; }
    public string? TemplatePath { get; internal init; }
    public TypeInfo? ReturnType { get; internal init; }
    public IReadOnlyCollection<RequestParameterDescriptor> Parameters { get; internal init; }

    protected LinkConfiguration(string rel, IDictionary<string, string>? parameterMappings = null)
    {
        Rel = rel.ToSnakeCase();
        ParameterMappings = parameterMappings ?? new Dictionary<string, string>();
    }

    public HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        if (Templated.HasValue && Templated.Value)
        {
            return new HyperMediaLink(httpContext.Request, TemplatePath, ReturnType, Parameters, Templated.Value);
        }

        var routeValues = BuildRouteValues(responseObjectProperties);
        var path = GenerateLink(httpContext, linkGenerator, routeValues);

        return new HyperMediaLink(httpContext.Request, path, ReturnType, Parameters);
    }

    protected abstract string? GenerateLink(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues);

    private ExpandoObject BuildRouteValues(IDictionary<string, object?> responseObjectProperties)
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

public class RequestParameterDescriptor
{
    public ParameterInfo? ParameterInfo { get; init; }
    public string? BindingSource { get; init; }
}