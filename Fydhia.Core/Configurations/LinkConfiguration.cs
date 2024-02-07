using System.Dynamic;
using System.Reflection;
using Fydhia.Core.Common;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Configurations;

public abstract class LinkConfiguration
{
    private readonly IDictionary<string, string> _parameterMappings;

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
    public string HttpMethod { get; internal init; }
    public IReadOnlyCollection<RequestParameterDescriptor> Parameters { get; internal init; }


    public const string SelfLinkRel = "self";

    protected LinkConfiguration(string rel, IDictionary<string, string>? parameterMappings = null)
    {
        Rel = rel.ToSnakeCase();
        _parameterMappings = parameterMappings ?? new Dictionary<string, string>();
    }

    public HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        if (Templated.HasValue && Templated.Value)
        {
            return new HyperMediaLink(httpContext.Request, TemplatePath, ReturnType, Parameters, HttpMethod, Templated.Value);
        }

        var routeValues = BuildRouteValues(responseObjectProperties);
        var path = GenerateLink(httpContext, linkGenerator, routeValues);

        return new HyperMediaLink(httpContext.Request, path, ReturnType, Parameters, HttpMethod);
    }

    protected abstract string? GenerateLink(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues);

    private ExpandoObject BuildRouteValues(IDictionary<string, object?> responseObjectProperties)
    {
        var result = new ExpandoObject();
        foreach (var parameterMapping in _parameterMappings)
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