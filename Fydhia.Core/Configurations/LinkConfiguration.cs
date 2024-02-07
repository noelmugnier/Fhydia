using System.Dynamic;
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

    public ParsedEndpoint ParsedEndpoint { get; }
    public string Rel { get; }
    public string? Title { get; internal init; }
    public string? Name { get; internal init; }
    public bool? Templated { get; internal init; }


    public const string SelfLinkRel = "self";

    protected LinkConfiguration(string rel, ParsedEndpoint parsedEndpoint, IDictionary<string, string>? parameterMappings = null)
    {
        Rel = rel.ToSnakeCase();
        ParsedEndpoint = parsedEndpoint;
        _parameterMappings = parameterMappings ?? new Dictionary<string, string>();
    }

    public HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        if (Templated.HasValue && Templated.Value)
        {
            return new HyperMediaLink(httpContext.Request, ParsedEndpoint.TemplatePath, ParsedEndpoint.ReturnedType, ParsedEndpoint.Parameters, ParsedEndpoint.HttpMethod, Templated.Value);
        }

        var routeValues = BuildRouteValues(responseObjectProperties);
        var path = GenerateLink(httpContext, linkGenerator, routeValues);

        return new HyperMediaLink(httpContext.Request, path, ParsedEndpoint.ReturnedType, ParsedEndpoint.Parameters, ParsedEndpoint.HttpMethod);
    }

    protected abstract string GenerateLink(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues);

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