using Fydhia.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fydhia.Core.Configurations;

public class NamedLinkConfiguration : LinkConfiguration
{
    private readonly string _endpointName;

    private NamedLinkConfiguration(string rel, string endpointName, IDictionary<string, string>? parameterMappings = null,
        string name = "", string title = "", bool templated = false)
        : base(rel, parameterMappings, name, title, templated)
    {
        _endpointName = endpointName;
    }

    internal static NamedLinkConfiguration Create(string? rel, string endpointName,
        IDictionary<string, string> parameterMappings, string name = "", string title = "", bool templated = false)
    {
        if (string.IsNullOrWhiteSpace(endpointName))
        {
            throw new ArgumentException($"Endpoint name must be provided to build a link configuration");
        }

        return new NamedLinkConfiguration(rel ?? endpointName, endpointName, parameterMappings, name, title, templated);
    }

    public override HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        var routeBuilder = httpContext.RequestServices.GetRequiredService<EndpointDataSource>();

        var routeEndpoint = GetRouteEndpoint(routeBuilder, _endpointName);
        if (routeEndpoint == null)
        {
            throw new InvalidOperationException($"Endpoint with name {_endpointName} not found");
        }

        if (Templated)
        {
            return new HyperMediaLink($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{routeEndpoint.RoutePattern.RawText}", Templated);
        }

        var routeValues = BuildActionRouteValues(responseObjectProperties);
        var path = linkGenerator.GetUriByName(httpContext, _endpointName, routeValues,
            options: new LinkOptions { LowercaseUrls = true, LowercaseQueryStrings = true });
        
        return new HyperMediaLink(path);
    }

    private RouteEndpoint? GetRouteEndpoint(EndpointDataSource routeBuilder, string endpointName)
    {
        foreach (var endpoint in routeBuilder.Endpoints)
        {
            var endpointNameMetadata = endpoint.Metadata.GetMetadata<EndpointNameMetadata>();
            var endpointNameAttribute = endpoint.Metadata.GetMetadata<EndpointNameAttribute>();
            if (endpointNameMetadata == null && endpointNameAttribute == null)
                continue;

            if (endpointNameMetadata?.EndpointName != endpointName &&
                endpointNameAttribute?.EndpointName != endpointName)
                continue;

            return endpoint as RouteEndpoint;
        }

        return null;
    }
}