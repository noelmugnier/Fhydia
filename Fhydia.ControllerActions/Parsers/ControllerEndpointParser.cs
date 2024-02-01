using System.Reflection;
using Fhydia.ControllerActions.Resources;
using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Fhydia.ControllerActions.Parsers;

public interface IParseEndpoints
{
    IEnumerable<EndpointResource> Parse();
    IEnumerable<EndpointResource> Parse(string methodName);
}

public class ControllerEndpointsParser<TController> : IParseEndpoints where TController : Controller
{
    private readonly IDictionary<string, IEnumerable<ControllerEndpoint>?> _endpoints = new Dictionary<string, IEnumerable<ControllerEndpoint>?>();

    public IEnumerable<EndpointResource> Parse()
    {
        return ParseEndpoints(typeof(TController).GetTypeInfo());
    }

    public IEnumerable<EndpointResource> Parse(string methodName)
    {
        var controllerType = typeof(TController);
        return ParseControllerMethod(controllerType.GetTypeInfo(), controllerType.GetMethod(methodName)!);
    }

    private IEnumerable<ControllerEndpoint> ParseEndpoints(TypeInfo controllerTypeInfo)
    {
        var operations = new List<ControllerEndpoint>();

        var controllerMethods = controllerTypeInfo
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList();
        if (controllerTypeInfo.BaseType != typeof(Controller) && controllerTypeInfo.BaseType != typeof(ControllerBase))
        {
            controllerMethods.AddRange(
                controllerTypeInfo.BaseType.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                       BindingFlags.DeclaredOnly));
        }

        foreach (var controllerMethod in controllerMethods)
        {
            var parsedEndpoints = ParseControllerMethod(controllerTypeInfo, controllerMethod);
            if (!parsedEndpoints.Any())
                continue;

            operations.AddRange(parsedEndpoints);
        }

        return operations;
    }

    private IEnumerable<ControllerEndpoint> ParseControllerMethod(TypeInfo controllerTypeInfo, MethodInfo controllerMethod)
    {
        var controllerMethodIdentifier = GetControllerEndpointIdentifier(controllerMethod, controllerTypeInfo);
        if(_endpoints.TryGetValue(controllerMethodIdentifier, out var existingEndpoints))
        {
            return existingEndpoints ?? Enumerable.Empty<ControllerEndpoint>();
        }

        if (controllerTypeInfo.IsAbstract || controllerTypeInfo.GetCustomAttribute<NonControllerAttribute>() != null)
        {
            _endpoints.TryAdd(controllerMethodIdentifier, null);
            return Enumerable.Empty<ControllerEndpoint>();
        }

        if (controllerMethod.GetCustomAttribute<NonActionAttribute>() != null)
        {
            _endpoints.TryAdd(controllerMethodIdentifier, null);
            return Enumerable.Empty<ControllerEndpoint>();
        }

        var endpoints = ControllerEndpoint.Create(controllerTypeInfo, controllerMethod);
        _endpoints.TryAdd(controllerMethodIdentifier, endpoints);
        return endpoints;
    }

    private static string GetControllerEndpointIdentifier(MethodInfo methodInfo, TypeInfo controllerType)
    {
        return $"{controllerType.FullName}.{methodInfo.Name}";
    }

    // private static string GetControllerEndpointIdentifier(string methodName)
    // {
    //     var controllerType = typeof(TController).GetTypeInfo();
    //     var methodInfo = controllerType.GetMethod(methodName);
    //     if (methodInfo == null)
    //     {
    //         throw new InvalidOperationException(
    //             $"Cannot find method '{methodName}' on Controller '{controllerType.Name}'");
    //     }
    //
    //     return GetControllerEndpointIdentifier(methodInfo, controllerType);
    // }


    // private static Uri ParseTemplates(MethodInfo methodInfo, TypeInfo controllerType)
    // {
    //     var httpAttributeTemplate =
    //         methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault()?.Template?.Trim();
    //     var routeAttributeTemplate =
    //         methodInfo.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault()?.Template?.Trim();
    //     var controllerRouteAttributeTemplate = controllerType.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault()
    //         ?.Template?.Trim();
    //
    //     if (httpAttributeTemplate != null && routeAttributeTemplate != null)
    //     {
    //         throw new InvalidOperationException(
    //             $"Cannot have HttpAttribute template '{httpAttributeTemplate}' and RouteAttribute template '{routeAttributeTemplate}' on Method '{methodInfo.Name}' for Controller '{controllerType.Name}'");
    //     }
    //
    //     if (controllerRouteAttributeTemplate != null && httpAttributeTemplate == null && routeAttributeTemplate == null)
    //     {
    //         throw new InvalidOperationException(
    //             $"Cannot have Method '{methodInfo.Name}' without HttpAttribute or RouteAttribute template for Controller '{controllerType.Name}' when using RouteAttribute on Controller");
    //     }
    //
    //     var templateUri = GetTemplateUri(httpAttributeTemplate, controllerRouteAttributeTemplate);
    //     if (templateUri != null)
    //     {
    //         return FormatTemplate(templateUri.ToString(), controllerType.GetControllerClassName(), methodInfo.Name);
    //     }
    //
    //     templateUri = GetTemplateUri(routeAttributeTemplate, controllerRouteAttributeTemplate);
    //     if (templateUri != null)
    //     {
    //         return FormatTemplate(templateUri.ToString(), controllerType.GetControllerClassName(), methodInfo.Name);
    //     }
    //
    //     if (controllerRouteAttributeTemplate == null && httpAttributeTemplate == null && routeAttributeTemplate == null)
    //     {
    //         return new Uri((controllerType.GetControllerClassName() + "/" + methodInfo.Name).Trim('/'),
    //             UriKind.Relative);
    //     }
    //
    //     throw new InvalidOperationException();
    // }

    // private static Uri? GetTemplateUri(string template, string controllerRouteTemplate)
    // {
    //     if (template == null)
    //     {
    //         return null;
    //     }
    //
    //     if (template.StartsWith("~/") || template.StartsWith("/"))
    //     {
    //         return new Uri(template.TrimStart('~').Trim('/'), UriKind.Relative);
    //     }
    //
    //     return new Uri($"{controllerRouteTemplate}/{template}".Trim('/'), UriKind.Relative);
    // }
    //
    // private static Uri FormatTemplate(string template, string controllerName, string methodName)
    // {
    //     var finalTemplate = template;
    //     if (template.Contains("[controller]"))
    //     {
    //         finalTemplate = template.Replace("[controller]", controllerName);
    //     }
    //
    //     if (template.Contains("[action]"))
    //     {
    //         finalTemplate = finalTemplate.Replace("[action]", methodName);
    //     }
    //
    //     return new Uri(finalTemplate, UriKind.Relative);
    // }

}