using System.Dynamic;
using Fydhia.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Configurations;

public class ActionLinkConfiguration<TControllerType> : LinkConfiguration
    where TControllerType : Controller
{
    private readonly string _controllerName;
    private readonly string _methodName;

    internal ActionLinkConfiguration(string? rel, EndpointInfo endpointInfo, string methodName,
        string controllerName,
        IDictionary<string, string>? parameterMappings = null,
        IDictionary<string, string>? headerMappings = null)
        : base(rel ?? methodName, endpointInfo, parameterMappings, headerMappings)
    {
        _controllerName = controllerName;
        _methodName = methodName;
    }

    protected override string GenerateLink(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues)
    {
        var path = linkGenerator.GetPathByAction(httpContext, _methodName, _controllerName, routeValues, options: LinkOptions);
        if(string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException($"Could not generate path for action {_methodName} on controller {_controllerName}");
        }

        return path;
    }
}