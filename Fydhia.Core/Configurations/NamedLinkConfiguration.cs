using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Configurations;

public class NamedLinkConfiguration : LinkConfiguration
{
    private readonly string _endpointName;

    internal NamedLinkConfiguration(string? rel, string endpointName, IDictionary<string, string>? parameterMappings = null)
        : base(rel ?? endpointName, parameterMappings)
    {
        _endpointName = endpointName;
    }

    protected override string? GenerateLink(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues)
    {
        return linkGenerator.GetPathByName(httpContext, _endpointName, routeValues, options: LinkOptions);
    }
}