using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Fydhia.Library;

public class ControllerEndpoint : EndpointResource
{
    public string ControllerName { get; }
    public string MethodName { get; }

    private ControllerEndpoint(string methodName, string controllerName, MethodInfo methodInfo, TypeInfo controllerType, HttpVerb verb,
        ControllerGroup group, ReturnedType result, IEnumerable<ParsedParameter> parameters) : base($"{controllerType.FullName}.{methodName}", controllerType, verb, group, result, parameters)
    {
        ControllerName = controllerName;
        MethodName = methodName;
        Description = methodInfo.GetTypeDescription();
    }

    public static IEnumerable<ControllerEndpoint> Create(TypeInfo controllerTypeInfo, MethodInfo controllerMethod)
    {
        var methodName = ParseMethodName(controllerMethod);
        var controllerName = controllerTypeInfo.GetControllerClassName();
        var verbs = ParseHttpVerbs(controllerMethod);
        var parameters = ParseParameters(controllerMethod);

        var group = ControllerGroup.CreateFrom(controllerMethod, controllerTypeInfo);
        var result = ReturnedType.CreateFrom(controllerMethod, controllerTypeInfo);

        return verbs.Select(verb => new ControllerEndpoint(methodName, controllerName, controllerMethod, controllerTypeInfo, verb, group, result,
            parameters));
    }

    private static string ParseMethodName(MethodInfo methodInfo)
    {
        // var httpAttributeName =
        //     methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault()?.Name?.Trim();
        //var routeAttributeName = methodInfo.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault()?.Name?.Trim();
        var actionAttributeName =
            methodInfo.GetCustomAttributes<ActionNameAttribute>(true).FirstOrDefault()?.Name?.Trim();

        // if (!string.IsNullOrWhiteSpace(httpAttributeName))
        // {
        //     return httpAttributeName;
        // }

        // if (!string.IsNullOrWhiteSpace(routeAttributeName))
        // {
        //     return routeAttributeName;
        // }

        if (!string.IsNullOrWhiteSpace(actionAttributeName))
        {
            return actionAttributeName;
        }

        return methodInfo.Name;
    }

    private static IEnumerable<ParsedParameter> ParseParameters(MethodInfo methodInfo)
    {
        var parameters = new List<ParsedParameter>();
        methodInfo.GetParameters().ToList().ForEach(p => parameters.Add(new ParsedParameter(p)));
        return parameters;
    }

    private static IEnumerable<HttpVerb> ParseHttpVerbs(MethodInfo methodInfo)
    {
        var httpMethodAttributes = methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).ToList();
        if (!httpMethodAttributes.Any())
        {
            return new[] { HttpVerb.GET };
        }

        return httpMethodAttributes.SelectMany(attr => attr.HttpMethods).Select(httpMethod => Enum.Parse<HttpVerb>(httpMethod));
    }
}