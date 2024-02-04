using System.Dynamic;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fhydia.Controllers;

public class ActionLinkConfiguration<TControllerType> : LinkConfiguration
    where TControllerType : Controller
{
    private readonly string _controllerName;
    private readonly string _methodName;

    internal ActionLinkConfiguration(string? rel, string methodName, string controllerName,
        IDictionary<string, string>? parameterMappings = null)
        : base(rel ?? methodName, parameterMappings)
    {
        _controllerName = controllerName;
        _methodName = methodName;
    }

    protected override string? GenerateLink(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues)
    {
        return linkGenerator.GetPathByAction(httpContext, _methodName, _controllerName, routeValues, options: LinkOptions);
    }
}