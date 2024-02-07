using System.Dynamic;
using Fydhia.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Configurations;

public class NamedLinkConfiguration : LinkConfiguration
{
    private readonly string _endpointName;

    internal NamedLinkConfiguration(string? rel, ParsedEndpoint parsedEndpoint, string endpointName, IDictionary<string, string>? parameterMappings = null)
        : base(rel ?? endpointName, parsedEndpoint, parameterMappings)
    {
        _endpointName = endpointName;
    }

    protected override string GenerateLink(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues)
    {
        var path = linkGenerator.GetPathByName(httpContext, _endpointName, routeValues, options: LinkOptions);
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException($"Could not generate path for endpoint {_endpointName}");
        }

        return path;
    }
}