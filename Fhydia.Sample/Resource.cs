using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Fhydia.Sample;

public abstract class Resource
{
    protected Resource(string name, Type relatedType) : this(name, relatedType.GetTypeInfo())
    {
    }

    protected Resource(string name, TypeInfo relatedType)
    {
        Name = name;
        TypeInfo = relatedType;
    }

    public string? Description { get; protected set; }
    public string Name { get; }
    public string DisplayName { get; }
    public TypeInfo TypeInfo { get; }
}

public class ParsedResult : Resource
{
    public IEnumerable<ParsedProperty> Properties { get; }

    private ParsedResult(string name, Type type) : base(name, type)
    {
        Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(p => new ParsedProperty(p));
        Description = type.GetTypeDescription();
    }

    public static ParsedResult CreateFrom(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var returnedType = methodInfo.ReturnType;

        if (returnedType.IsGenericType)
        {
            returnedType = RecursivelyFindComplexOrSimpleType(returnedType);
        }

        if (returnedType == typeof(ActionResult) && !returnedType.IsGenericType)
        {
            var produceResponseTypeAttribute = methodInfo.GetCustomAttributes<ProducesResponseTypeAttribute>(true).FirstOrDefault(c => c.Type != null);
            if (produceResponseTypeAttribute == null)
            {
                throw new InvalidOperationException($"Cannot have {returnedType} returned type without ProducesResponseTypeAttribute on Method: {methodInfo.Name} for Controller: {controllerType.Name}");
            }
            else
            {
                returnedType = produceResponseTypeAttribute.Type;
            }
        }

        return new ParsedResult(returnedType.Name, returnedType);
    }

    private static Type RecursivelyFindComplexOrSimpleType(Type type)
    {
        if (!type.IsGenericType)
        {
            return type;
        }

        var returnedType = type;
        var genericReturnedType = returnedType.GetGenericTypeDefinition();
        if (returnedType.IsGenericType && (genericReturnedType == typeof(ActionResult<>) || genericReturnedType == typeof(Task<>) || genericReturnedType == typeof(ValueTask<>)))
        {
            returnedType = RecursivelyFindComplexOrSimpleType(returnedType.GenericTypeArguments[0]);
        }

        return returnedType;
    }
}

public class ParsedEndpoint : Resource
{
    public readonly MethodInfo MethodInfo;

    public Uri Template { get; }
    public HttpVerb Method { get; }
    public IEnumerable<ParsedParameter> Parameters { get; }
    public ParsedResult Result { get; }
    public ParsedGroup Group { get; }

    public ParsedEndpoint(string name, MethodInfo methodInfo, TypeInfo controllerType, Uri template, HttpVerb method, ParsedGroup group, ParsedResult result, IEnumerable<ParsedParameter> parameters) : base(name, controllerType)
    {
        MethodInfo = methodInfo;
        Template = template;
        Method = method;
        Group = group;
        Result = result;
        Parameters = parameters;
        Description = methodInfo.GetTypeDescription();
    }
}

public class ParsedGroup : Resource
{
    private ParsedGroup(MethodInfo methodInfo, TypeInfo controllerTypeInfo) : base(GetControllerGroupName(methodInfo, controllerTypeInfo), controllerTypeInfo)
    {
    }

    public static ParsedGroup CreateFrom(MethodInfo methodInfo, TypeInfo controllerTypeInfo)
    {
        return new ParsedGroup(methodInfo, controllerTypeInfo)
        {
            Description = controllerTypeInfo.GetTypeDescription()
        };
    }

    private static string GetControllerGroupName(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var controllerName = controllerType.GetControllerClassName();
        var methodApiExplorerSettingAttributeName = methodInfo.GetCustomAttributes<ApiExplorerSettingsAttribute>(true).FirstOrDefault()?.GroupName?.Trim();
        var controllerApiExplorerSettingAttributeName = controllerType.GetCustomAttributes<ApiExplorerSettingsAttribute>(true).FirstOrDefault()?.GroupName?.Trim();

        if (!string.IsNullOrWhiteSpace(methodApiExplorerSettingAttributeName))
        {
            return methodApiExplorerSettingAttributeName;
        }

        if (!string.IsNullOrWhiteSpace(controllerApiExplorerSettingAttributeName))
        {
            return controllerApiExplorerSettingAttributeName;
        }

        return controllerName;
    }
}

public class ParsedProperty : Resource
{
    public readonly PropertyInfo PropertyInfo;

    public ParsedProperty(PropertyInfo propertyInfo) : base(propertyInfo.Name, propertyInfo.PropertyType)
    {
        PropertyInfo = propertyInfo;
        Description = propertyInfo.GetTypeDescription();
    }
}

public class ParsedParameter : Resource
{
    public readonly ParameterInfo ParameterInfo;

    public ParsedParameter(ParameterInfo parameterInfo) : base(parameterInfo.Name, parameterInfo.ParameterType)
    {
        ParameterInfo = parameterInfo;
        BindingSource = ParseParameterAttribute(parameterInfo);
        Description = parameterInfo.GetTypeDescription();
    }

    private BindingSource? ParseParameterAttribute(ParameterInfo parameterInfo)
    {
        var availableAttributeTypes = new[] { typeof(FromQueryAttribute), typeof(FromRouteAttribute), typeof(FromBodyAttribute), typeof(FromHeaderAttribute), typeof(FromFormAttribute) };

        foreach (var attributeType in availableAttributeTypes)
        {
            var bindingSource = ExtractBindingSourceAttribute(parameterInfo, attributeType);
            if (bindingSource != null)
            {
                return bindingSource;
            }
        }

        return null;
    }

    private static BindingSource ExtractBindingSourceAttribute(ParameterInfo parameterInfo, Type bindingSourceMetadataAttribute)
    {
        var attribute = parameterInfo.GetCustomAttribute(bindingSourceMetadataAttribute);
        return attribute is not null and IBindingSourceMetadata bindingSourceMetadata ? bindingSourceMetadata.BindingSource : null;
    }

    public BindingSource? BindingSource { get; }
}

public class EndpointParser
{
    public IEnumerable<ParsedEndpoint> ParseControllerEndpoints(Type controllerType)
    {
        return ParseControllerEndpoints(controllerType.GetTypeInfo());
    }

    public IEnumerable<ParsedEndpoint> ParseControllerEndpoints<TController>() where TController : Controller
    {
        return ParseControllerEndpoints(typeof(TController).GetTypeInfo());
    }

    public IEnumerable<ParsedEndpoint> ParseControllerOperationEndpoints<TController>(string operationName) where TController : Controller
    {
        return ParseControllerEndpoints(typeof(TController).GetTypeInfo()).Where(c => c.MethodInfo.Name == operationName);
    }

    public IEnumerable<ParsedEndpoint> ParseControllerEndpoints(TypeInfo controllerTypeInfo)
    {
        var operations = new List<ParsedEndpoint>();
        if (controllerTypeInfo.IsAbstract || controllerTypeInfo.GetCustomAttribute<NonControllerAttribute>() != null)
        {
            return operations;
        }

        var controllerMethods = controllerTypeInfo.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList();
        if (controllerTypeInfo.BaseType != typeof(Controller) && controllerTypeInfo.BaseType != typeof(ControllerBase))
        {
            controllerMethods.AddRange(controllerTypeInfo.BaseType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
        }

        foreach (var controllerMethod in controllerMethods)
        {
            if (controllerMethod.GetCustomAttribute<NonActionAttribute>() != null)
            {
                continue;
            }

            var name = ParseName(controllerMethod, controllerTypeInfo);
            var templates = ParseTemplates(controllerMethod, controllerTypeInfo);
            var methods = ParseHttpMethods(controllerMethod);
            var parameters = ParseParameters(controllerMethod);

            var group = ParsedGroup.CreateFrom(controllerMethod, controllerTypeInfo);
            var result = ParsedResult.CreateFrom(controllerMethod, controllerTypeInfo);

            operations.Add(new ParsedEndpoint(name, controllerMethod, controllerTypeInfo, templates, methods, group, result, parameters));
        }

        return operations;
    }

    private static string ParseName(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var httpAttributeName = methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault()?.Name?.Trim();
        var routeAttributeName = methodInfo.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault()?.Name?.Trim();
        var actionAttributeName = methodInfo.GetCustomAttributes<ActionNameAttribute>(true).FirstOrDefault()?.Name?.Trim();

        if (!string.IsNullOrWhiteSpace(httpAttributeName))
        {
            return httpAttributeName;
        }

        if (!string.IsNullOrWhiteSpace(routeAttributeName))
        {
            return routeAttributeName;
        }

        if (!string.IsNullOrWhiteSpace(actionAttributeName))
        {
            return actionAttributeName;
        }

        return $"{controllerType.GetControllerClassName()}_{methodInfo.Name}";
    }

    private static Uri ParseTemplates(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var httpAttributeTemplate = methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault()?.Template?.Trim();
        var routeAttributeTemplate = methodInfo.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault()?.Template?.Trim();
        var controllerRouteAttributeTemplate = controllerType.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault()?.Template?.Trim();

        if (httpAttributeTemplate != null && routeAttributeTemplate != null)
        {
            throw new InvalidOperationException($"Cannot have HttpAttribute template '{httpAttributeTemplate}' and RouteAttribute template '{routeAttributeTemplate}' on Method '{methodInfo.Name}' for Controller '{controllerType.Name}'");
        }

        if (controllerRouteAttributeTemplate != null && httpAttributeTemplate == null && routeAttributeTemplate == null)
        {
            throw new InvalidOperationException($"Cannot have Method '{methodInfo.Name}' without HttpAttribute or RouteAttribute template for Controller '{controllerType.Name}' when using RouteAttribute on Controller");
        }

        var templateUri = GetTemplateUri(httpAttributeTemplate, controllerRouteAttributeTemplate);
        if (templateUri != null)
        {
            return templateUri;
        }

        templateUri = GetTemplateUri(routeAttributeTemplate, controllerRouteAttributeTemplate);
        if (templateUri != null)
        {
            return templateUri;
        }

        if (controllerRouteAttributeTemplate == null && httpAttributeTemplate == null && routeAttributeTemplate == null)
        {
            return new Uri((controllerType.GetControllerClassName() + "/" + methodInfo.Name).Trim('/'), UriKind.Relative);
        }

        throw new InvalidOperationException();
    }

    private static Uri? GetTemplateUri(string template, string controllerRouteTemplate)
    {
        if (template == null)
        {
            return null;
        }

        if (template.StartsWith("~/") || template.StartsWith("/"))
        {
            return new Uri(template.TrimStart('~').Trim('/'), UriKind.Relative);
        }

        return new Uri($"{controllerRouteTemplate}/{template}".Trim('/'), UriKind.Relative);
    }

    private static IEnumerable<ParsedParameter> ParseParameters(MethodInfo methodInfo)
    {
        var parameters = new List<ParsedParameter>();
        methodInfo.GetParameters().ToList().ForEach(p => parameters.Add(new ParsedParameter(p)));
        return parameters;
    }

    private static HttpVerb ParseHttpMethods(MethodInfo methodInfo)
    {
        var httpMethodAttribute = methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault();
        if (httpMethodAttribute == null)
        {
            return HttpVerb.GET;
        }

        var httpMethod = httpMethodAttribute.HttpMethods.FirstOrDefault();
        return Enum.Parse<HttpVerb>(httpMethod);
    }
}
