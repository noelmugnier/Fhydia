using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Fhydia.Sample;

public abstract class Resource
{
    protected Resource(string name, Type relatedType)
    {
        Name = name;
        RelatedType = relatedType;
    }

    public string? Description { get; }
    public string Name { get; }
    public string DisplayName { get; }
    public Type RelatedType { get; }
}

public class Result : Resource
{
    public readonly TypeInfo TypeInfo;

    private Result(TypeInfo typeInfo, string name) : base(name, typeInfo.AsType())
    {
        Properties = typeInfo.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(p => new Property(p));
        TypeInfo = typeInfo;
    }

    public static Result CreateFrom(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var returnedType = methodInfo.ReturnType;

        if (returnedType.IsGenericType)
        {
            returnedType = GetGenericInnerType(returnedType);
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

        return new Result(returnedType.GetTypeInfo(), returnedType.Name);
    }

    public IEnumerable<Property> Properties { get; }

    private static Type GetGenericInnerType(Type innerType)
    {
        if (!innerType.IsGenericType)
        {
            return innerType;
        }

        var returnedType = innerType;
        var genericReturnedType = returnedType.GetGenericTypeDefinition();
        if (returnedType.IsGenericType && (genericReturnedType == typeof(ActionResult<>) || genericReturnedType == typeof(Task<>) || genericReturnedType == typeof(ValueTask<>)))
        {
            returnedType = GetGenericInnerType(returnedType.GenericTypeArguments[0]);
        }

        return returnedType;
    }
}

public class Operation : Resource
{
    private readonly MethodInfo _methodInfo;

    private Operation(MethodInfo methodInfo, TypeInfo controllerType) : base(ParseName(methodInfo, controllerType), controllerType)
    {
        _methodInfo = methodInfo;
        Template = ParseTemplate(methodInfo, controllerType);
        Method = ParseHttpMethod(methodInfo);
        Result = Result.CreateFrom(methodInfo, controllerType);
        Parameters = ParseParameters(methodInfo);
    }

    private static string ParseName(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var httpMethodAttribute = methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault();
        var routeAttribute = methodInfo.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault();
        var actionNameAttribute = methodInfo.GetCustomAttributes<ActionNameAttribute>(true).FirstOrDefault();

        var httpMethodRoute = httpMethodAttribute?.Name?.Trim();
        var routeTemplate = routeAttribute?.Name?.Trim();
        var actionName = actionNameAttribute?.Name?.Trim();

        if (!string.IsNullOrWhiteSpace(httpMethodRoute))
        {
            return httpMethodRoute;
        }

        if (!string.IsNullOrWhiteSpace(routeTemplate))
        {
            return routeTemplate;
        }

        if (!string.IsNullOrWhiteSpace(actionName))
        {
            return actionName;
        }

        return $"{GetConventionControllerRoute(controllerType)}_{methodInfo.Name}";
    }

    private static Uri? ParseTemplate(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var httpMethodAttribute = methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault();
        var routeAttribute = methodInfo.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault();

        var httpMethodRoute = httpMethodAttribute?.Template?.Trim();
        var routeTemplate = routeAttribute?.Template?.Trim();
        var controllerRouteAttribute = controllerType.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault();

        if (httpMethodRoute != null && routeTemplate != null)
        {
            throw new InvalidOperationException($"Cannot have HttpAttribute template '{httpMethodRoute}' and RouteAttribute template '{routeTemplate}' on Method: {methodInfo.Name} for Controller {controllerType.Name}");
        }

        if (controllerRouteAttribute != null && httpMethodRoute == null && routeTemplate == null)
        {
            throw new InvalidOperationException($"Cannot have Method {methodInfo.Name} without HttpAttribute or RouteAttribute template for Controller {controllerType.Name}");
        }

        if (httpMethodRoute != null)
        {
            if (httpMethodRoute.StartsWith("~/") || httpMethodRoute.StartsWith("/"))
            {
                return new Uri(httpMethodRoute.TrimStart('~').Trim('/'), UriKind.Relative);
            }

            return new Uri($"{controllerRouteAttribute?.Template}/{httpMethodRoute}".Trim('/'), UriKind.Relative);
        }

        if (routeTemplate != null)
        {
            if (routeTemplate.StartsWith("~/") || routeTemplate.StartsWith("/"))
            {
                return new Uri(routeTemplate.TrimStart('~').Trim('/'), UriKind.Relative);
            }

            return new Uri($"{controllerRouteAttribute?.Template}/{routeTemplate}".Trim('/'), UriKind.Relative);
        }

        if (controllerRouteAttribute == null && httpMethodRoute == null && routeTemplate == null)
        {
            return new Uri((GetConventionControllerRoute(controllerType) + "/" + methodInfo.Name).Trim('/'), UriKind.Relative);
        }

        throw new InvalidOperationException();
    }

    private static string GetConventionControllerRoute(TypeInfo controllerType)
    {
        return $"{controllerType.Name.Replace("Controller", string.Empty)}";
    }

    private static IEnumerable<Parameter> ParseParameters(MethodInfo methodInfo)
    {
        var parameters = new List<Parameter>();
        methodInfo.GetParameters().ToList().ForEach(p => parameters.Add(new Parameter(p)));
        return parameters;
    }

    private static HttpVerb ParseHttpMethod(MethodInfo methodInfo)
    {
        var httpMethodAttribute = methodInfo.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault();
        if (httpMethodAttribute == null)
        {
            return HttpVerb.GET;
        }

        var httpMethod = httpMethodAttribute.HttpMethods.FirstOrDefault();
        return Enum.Parse<HttpVerb>(httpMethod);
    }

    public static Operation CreateFrom(MethodInfo methodInfo, TypeInfo controllerType)
    {
        return new Operation(methodInfo, controllerType);
    }

    public Uri Template { get; }
    public HttpVerb Method { get; }
    public IEnumerable<Parameter> Parameters { get; }
    public Result Result { get; }
    public string MethodName => _methodInfo.Name;
}

public class Property : Resource
{
    public readonly PropertyInfo PropertyInfo;

    public Property(PropertyInfo propertyInfo) : base(propertyInfo.Name, propertyInfo.PropertyType)
    {
        PropertyInfo = propertyInfo;
    }
}

public class Parameter : Resource
{
    public readonly ParameterInfo ParameterInfo;

    public Parameter(ParameterInfo parameterInfo) : base(parameterInfo.Name, parameterInfo.ParameterType)
    {
        ParameterInfo = parameterInfo;
        BindingSource = ParseParameterAttribute(parameterInfo);
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

public class ParsedHypermedia
{
    public IEnumerable<Result> Results { get; set; }
    public IEnumerable<Operation> Operations { get; set; }
}

public class Processor
{
    public IEnumerable<Operation> ParseController(TypeInfo controllerType)
    {
        var operations = new List<Operation>();
        var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        foreach (var method in methods)
        {
            var operation = Operation.CreateFrom(method, controllerType);
            operations.Add(operation);
        }
        return operations;
    }
}
