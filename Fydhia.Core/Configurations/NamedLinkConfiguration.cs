using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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

    protected override string? GenerateNonTemplatedPath(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues, LinkOptions linkOptions)
    {
        return linkGenerator.GetUriByName(httpContext, _endpointName, routeValues, options: linkOptions);
    }

    protected override RouteEndpoint? GetRouteEndpoint(EndpointDataSource routeBuilder)
    {
        foreach (var endpoint in routeBuilder.Endpoints)
        {
            var endpointNameMetadata = endpoint.Metadata.GetMetadata<EndpointNameMetadata>();
            var endpointNameAttribute = endpoint.Metadata.GetMetadata<EndpointNameAttribute>();
            if (endpointNameMetadata == null && endpointNameAttribute == null)
                continue;

            if (endpointNameMetadata?.EndpointName != _endpointName &&
                endpointNameAttribute?.EndpointName != _endpointName)
                continue;

            return endpoint as RouteEndpoint;
        }

        return null;
    }
}