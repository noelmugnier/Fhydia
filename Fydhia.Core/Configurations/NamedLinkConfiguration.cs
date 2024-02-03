using Fydhia.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Configurations;

public class NamedLinkConfiguration : LinkConfiguration
{
    private readonly string _name;

    private NamedLinkConfiguration(string rel, string name, IDictionary<string, string>? parameterMappings = null)
        : base(rel, parameterMappings)
    {
        _name = name;
    }

    internal static NamedLinkConfiguration Create(string? rel, string name, 
        IDictionary<string, string> parameterMappings)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"Name must be provided to build a link configuration");
        }

        return new NamedLinkConfiguration(rel ?? name, name, parameterMappings);
    }

    public override HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        var routeValues = BuildActionRouteValues(responseObjectProperties);
        var path = linkGenerator.GetUriByName(httpContext, _name, routeValues,
            options: new LinkOptions { LowercaseUrls = true, LowercaseQueryStrings = true });
        
        return new HyperMediaLink(path);
    }
}