using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class ControllerEndpoint : EndpointResource
{
    public string ControllerName { get; }
    public string MethodName { get; }

    private ControllerEndpoint(string methodName, string controllerName, MethodInfo methodInfo, TypeInfo controllerType, HttpVerb verb,
        ControllerGroup group, ControllerEndpointResult result, IEnumerable<ParsedParameter> parameters) : base($"{controllerType.FullName}.{methodName}", controllerType, verb, group, result, parameters)
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
        var result = ControllerEndpointResult.CreateFrom(controllerMethod, controllerTypeInfo);

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

    public override HyperMediaLink GenerateHyperMediaLink(LinkGenerator linkGenerator,
        IDictionary<string, string> parametersMapping,
        HttpContext httpContext, IDictionary<string, object?> values)
    {
        var result = new ExpandoObject();
        foreach (var mapper in parametersMapping)
        {
            if (!values.TryGetValue(mapper.Value, out var value))
            {
                continue;
            }

            result.TryAdd(mapper.Key, value);
        }

        var path = linkGenerator.GetUriByAction(httpContext, MethodName, ControllerName,
            result, options: new LinkOptions { LowercaseUrls = true, LowercaseQueryStrings = true });

        return new HyperMediaLink(path, Verb.ToString());
    }
}