using System.Reflection;
using Fhydia.Controllers.Extensions;
using Fydhia.Core.Common;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fhydia.Controllers.ControllerLink;

public class ControllerLinkConfiguration<TControllerType> : LinkConfiguration
    where TControllerType : Controller
{
    private readonly string _controllerName;
    private readonly string _methodName;

    private ControllerLinkConfiguration(string? rel, string methodName, string controllerName, IDictionary<string, string>? parameterMappings = null) 
        : base(rel ?? methodName, parameterMappings)
    {
        _controllerName = controllerName;
        _methodName = methodName;
    }

    internal static ControllerLinkConfiguration<TControllerType> Create(string? rel, string methodName,
        IDictionary<string, string> parameterMappings)
    {
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException(
                $"Method name must be provided to build a link configuration for controller {typeof(TControllerType).FullName}");
        }

        var controllerName = typeof(TControllerType).GetTypeInfo().GetControllerClassName();
        return new ControllerLinkConfiguration<TControllerType>(rel, methodName, controllerName, parameterMappings);
    }

    public override HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        var routeValues = BuildActionRouteValues(responseObjectProperties);
        var path = linkGenerator.GetUriByAction(httpContext, _methodName, _controllerName, routeValues,
            options: new LinkOptions { LowercaseUrls = true, LowercaseQueryStrings = true });

        return new HyperMediaLink(path);
    }
}