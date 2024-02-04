using System.Reflection;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace Fhydia.Controllers;

public class RouteEndpointParser
{
    public IReadOnlyCollection<RequestParameterDescriptor> GetParameters(RouteEndpoint routeEndpoint)
    {
        var requestParameterDescriptors = new List<RequestParameterDescriptor>();
        var controllerActionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor is null)
            return requestParameterDescriptors;

        foreach (var parameter in controllerActionDescriptor.Parameters)
        {
            if(parameter is not ControllerParameterDescriptor parameterDescriptor)
                continue;

            requestParameterDescriptors.Add(new RequestParameterDescriptor()
            {
                ParameterInfo = parameterDescriptor.ParameterInfo,
                BindingSource = parameterDescriptor.BindingInfo?.BindingSource?.Id
            });
        }

        return requestParameterDescriptors;
    }

    public TypeInfo GetReturnedType(RouteEndpoint routeEndpoint)
    {
        var producesResponseTypeMetadata = routeEndpoint.Metadata.GetMetadata<IProducesResponseTypeMetadata>();
        if (producesResponseTypeMetadata?.Type != null)
        {
            return producesResponseTypeMetadata.Type.GetTypeInfo();
        }

        var producesAttribute = routeEndpoint.Metadata.GetMetadata<ProducesAttribute>();
        if (producesAttribute?.Type != null)
        {
            return producesAttribute.Type.GetTypeInfo();
        }

        var produceResponseAttribute = routeEndpoint.Metadata.GetMetadata<ProducesResponseTypeAttribute>();
        if (produceResponseAttribute?.Type != null)
        {
            return produceResponseAttribute.Type.GetTypeInfo();
        }

        var controllerActionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor is null)
            return typeof(object).GetTypeInfo();

        return GetMethodConcreteReturnType(controllerActionDescriptor.MethodInfo);
    }

    private static TypeInfo GetMethodConcreteReturnType(MethodInfo methodInfo)
    {
        var returnType = methodInfo.ReturnType;
        if (returnType.IsGenericType)
        {
            returnType = RecursivelyFindComplexOrSimpleType(returnType);
        }

        if (returnType.IsGenericType || _nonSupportedNonGenericTypes.Any(typeToSkip => typeToSkip == returnType))
        {
            return returnType.GetTypeInfo();
        }

        var produceResponseTypeAttribute = methodInfo
            .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
            .FirstOrDefault();

        if (produceResponseTypeAttribute != null)
        {
            returnType = produceResponseTypeAttribute.Type;
        }

        return returnType.GetTypeInfo();
    }

    private static Type RecursivelyFindComplexOrSimpleType(Type type)
    {
        var returnType = type;
        if (!returnType.IsGenericType)
        {
            return returnType;
        }

        var genericReturnType = returnType.GetGenericTypeDefinition();
        if (returnType.IsGenericType &&_genericTypesToRecurse.Any(genericTypeToRecurse => genericTypeToRecurse == genericReturnType))
        {
            var returnTypeArgument = returnType.GenericTypeArguments.Length == 1
                ? returnType.GenericTypeArguments[0]
                : returnType.GenericTypeArguments.FirstOrDefault(typeArgument =>
                    typeArgument.IsGenericType && typeArgument.GetGenericTypeDefinition() == typeof(Ok<>));

            returnType = RecursivelyFindComplexOrSimpleType(returnTypeArgument);
        }

        return returnType;
    }

    private static Type[] _nonSupportedNonGenericTypes = new[]
    {
        typeof(ActionResult),
        typeof(IActionResult),
        typeof(Results),
        typeof(IResult),
        typeof(Task),
        typeof(ValueTask),
        typeof(void),
        typeof(OkObjectResult),
        typeof(ObjectResult)
    };

    private static Type[] _genericTypesToRecurse = new[]
    {
        typeof(ActionResult<>),
        typeof(Results<,>),
        typeof(Results<,,>),
        typeof(Results<,,,>),
        typeof(Results<,,,,>),
        typeof(Results<,,,,,>),
        typeof(Ok<>),
        typeof(Task<>),
        typeof(ValueTask<>)
    };
}