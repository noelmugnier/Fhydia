using System.Reflection;
using Fhydia.Controllers.Extensions;
using Fydhia.Core.Common;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fhydia.Controllers;

public class ActionLinkConfiguration<TControllerType> : LinkConfiguration
    where TControllerType : Controller
{
    private readonly string _controllerName;
    private readonly string _methodName;

    private ActionLinkConfiguration(string? rel, string methodName, string controllerName,
        IDictionary<string, string>? parameterMappings = null, string name = "", string title = "", bool templated = false)
        : base(rel ?? methodName, parameterMappings, name, title, templated)
    {
        _controllerName = controllerName;
        _methodName = methodName;
    }

    internal static ActionLinkConfiguration<TControllerType> Create(string? rel, string methodName,
        IDictionary<string, string> parameterMappings, string name, string title, bool templated)
    {
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException(
                $"Method name must be provided to build a link configuration for controller {typeof(TControllerType).FullName}");
        }

        var controllerName = typeof(TControllerType).GetTypeInfo().GetControllerClassName();
        return new ActionLinkConfiguration<TControllerType>(rel, methodName, controllerName, parameterMappings, name, title, templated);
    }

    public override HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        var routeBuilder = httpContext.RequestServices.GetRequiredService<EndpointDataSource>();

        var routeEndpoint = GetRouteEndpoint(routeBuilder, _methodName, _controllerName);
        if (routeEndpoint == null)
        {
            throw new InvalidOperationException($"Endpoint with method {_methodName} on controller {_controllerName} not found");
        }

        if (Templated)
        {
            return new HyperMediaLink($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{routeEndpoint.RoutePattern.RawText}", Templated);
        }

        var routeValues = BuildActionRouteValues(responseObjectProperties);
        var path = linkGenerator.GetUriByAction(httpContext, _methodName, _controllerName, routeValues,
            options: new LinkOptions { LowercaseUrls = true, LowercaseQueryStrings = true });

        return new HyperMediaLink(path);
    }

    private RouteEndpoint? GetRouteEndpoint(EndpointDataSource routeBuilder, string methodName, string controllerName)
    {
        var routeEndpoint = routeBuilder.Endpoints.FirstOrDefault(endpoint =>
            endpoint.Metadata.GetMetadata<ControllerActionDescriptor>().ControllerName == controllerName
            && endpoint.Metadata.GetMetadata<ControllerActionDescriptor>().MethodInfo.Name == methodName) as RouteEndpoint;

        return routeEndpoint;
    }
}