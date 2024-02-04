using System.Dynamic;
using Fydhia.Core.Common;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fydhia.Core.Configurations;

public abstract class LinkConfiguration
{
    protected readonly IDictionary<string, string> ParameterMappings;

    public readonly string Rel;
    public readonly string? Title;
    public readonly string? Name;
    public readonly bool Templated;

    protected LinkConfiguration(string rel, IDictionary<string, string>? parameterMappings = null,
        string name = "", string title = "", bool templated = false)
    {
        Name = name;
        Title = title;
        Templated = templated;
        Rel = rel.ToSnakeCase();
        ParameterMappings = parameterMappings ?? new Dictionary<string, string>();
    }

    public HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        var routeBuilder = httpContext.RequestServices.GetRequiredService<EndpointDataSource>();

        var routeEndpoint = GetRouteEndpoint(routeBuilder);
        if (routeEndpoint == null)
        {
            throw new InvalidOperationException($"RouteEndpoint not found");
        }

        if (Templated)
        {
            return new HyperMediaLink(httpContext.Request, routeEndpoint.RoutePattern.RawText, Templated);
        }

        var routeValues = BuildRouteValues(responseObjectProperties);
        var path = GenerateNonTemplatedPath(httpContext, linkGenerator, routeValues, new LinkOptions
        {
            LowercaseUrls = true,
            LowercaseQueryStrings = true,
            AppendTrailingSlash = false
        });

        return new HyperMediaLink(httpContext.Request, path);
    }

    protected abstract string? GenerateNonTemplatedPath(HttpContext httpContext, LinkGenerator linkGenerator,
        ExpandoObject routeValues, LinkOptions linkOptions);

    protected abstract RouteEndpoint? GetRouteEndpoint(EndpointDataSource routeBuilder);

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