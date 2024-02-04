using System.Dynamic;
using System.Reflection;
using Fhydia.Controllers.Extensions;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

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

        var controllerTypeInfo = typeof(TControllerType).GetTypeInfo();
        var method = controllerTypeInfo.GetMethod(methodName);
        if(method is null)
        {
            throw new ArgumentException(
                $"Method {methodName} not found on controller {typeof(TControllerType).FullName}");
        }

        var controllerName = controllerTypeInfo.GetControllerClassName();
        return new ActionLinkConfiguration<TControllerType>(rel, methodName, controllerName, parameterMappings, name, title, templated);
    }

    protected override string? GenerateNonTemplatedPath(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues,
        LinkOptions linkOptions)
    {
        return linkGenerator.GetPathByAction(httpContext, _methodName, _controllerName, routeValues, options: linkOptions);
    }

    protected override RouteEndpoint? GetRouteEndpoint(EndpointDataSource routeBuilder)
    {
        var routeEndpoint = routeBuilder.Endpoints.FirstOrDefault(endpoint =>
            endpoint.Metadata.GetMetadata<ControllerActionDescriptor>().ControllerName == _controllerName
            && endpoint.Metadata.GetMetadata<ControllerActionDescriptor>().MethodInfo.Name == _methodName) as RouteEndpoint;

        return routeEndpoint;
    }
}