using System.Reflection;
using System.Security.Claims;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace Fhydia.Controllers;

public class RouteEndpointParser
{
    public IReadOnlyCollection<RequestParameterDescriptor> GetParameters(RouteEndpoint routeEndpoint)
    {
        var requestParameterDescriptors = new List<RequestParameterDescriptor>();
        var methodInfo = routeEndpoint.Metadata.GetMetadata<MethodInfo>();
        if (methodInfo is not null)
        {
            return GetMinimalApiEndpointParameters(methodInfo, requestParameterDescriptors);
        }

        var controllerActionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        return controllerActionDescriptor is null
            ? requestParameterDescriptors
            : GetControllerEndpointParameters(controllerActionDescriptor, requestParameterDescriptors);
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

        var methodInfo = routeEndpoint.Metadata.GetMetadata<MethodInfo>();
        if (methodInfo is not null)
        {
            return GetMethodConcreteReturnType(methodInfo);
        }

        var controllerActionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor is null)
        {
            return typeof(object).GetTypeInfo();
        }

        return GetMethodConcreteReturnType(controllerActionDescriptor.MethodInfo);
    }

    private static TypeInfo GetMethodConcreteReturnType(MethodInfo methodInfo)
    {
        var returnType = methodInfo.ReturnType;
        if (returnType.IsGenericType)
        {
            returnType = RecursivelyFindComplexOrSimpleType(returnType);
        }

        if (returnType is not null && (returnType.IsGenericType || NonSupportedNonGenericTypes.Any(typeToSkip => typeToSkip == returnType)))
        {
            return returnType.GetTypeInfo();
        }

        var produceResponseTypeAttribute = methodInfo
            .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
            .FirstOrDefault();

        returnType = produceResponseTypeAttribute?.Type ?? methodInfo.ReturnType;
        return returnType.GetTypeInfo();
    }

    private static IReadOnlyCollection<RequestParameterDescriptor> GetControllerEndpointParameters(
        ControllerActionDescriptor controllerActionDescriptor,
        List<RequestParameterDescriptor> requestParameterDescriptors)
    {
        var controllerParameterDescriptors = controllerActionDescriptor.Parameters
            .OfType<ControllerParameterDescriptor>()
            .Where(controllerParameterDescriptor => ParameterIsFromRequestValue(controllerParameterDescriptor.ParameterInfo))
            .Select(controllerParameterDescriptor => new RequestParameterDescriptor
            {
                ParameterInfo = controllerParameterDescriptor.ParameterInfo,
                BindingSource = controllerParameterDescriptor.BindingInfo?.BindingSource,
                BinderModelName = controllerParameterDescriptor.BindingInfo?.BinderModelName ?? controllerParameterDescriptor.Name
            });

        requestParameterDescriptors.AddRange(controllerParameterDescriptors);

        return requestParameterDescriptors;
    }

    private IReadOnlyCollection<RequestParameterDescriptor> GetMinimalApiEndpointParameters(
        MethodInfo methodInfo,
        List<RequestParameterDescriptor> requestParameterDescriptors)
    {
        var parameterDescriptors = methodInfo
            .GetParameters()
            .Where(ParameterIsFromRequestValue)
            .Select(parameterInfo =>
            {
                var parameterBinding = GetParameterBinding(parameterInfo);
                return new RequestParameterDescriptor
                {
                    ParameterInfo = parameterInfo,
                    BindingSource = parameterBinding.BindingSource,
                    BinderModelName = parameterBinding.BinderModelName
                };
            });

        requestParameterDescriptors.AddRange(parameterDescriptors);

        return requestParameterDescriptors;
    }

    private static bool ParameterIsFromRequestValue(ParameterInfo parameter)
    {
        return parameter.GetCustomAttributes().OfType<IBindingSourceMetadata>().All(bindingSource => bindingSource.BindingSource != BindingSource.Services) &&
               DefaultMinimalApiInjectedTypes.All(type => type != parameter.ParameterType);
    }

    private ParameterBinding GetParameterBinding(ParameterInfo parameterInfo)
    {
        var bindingSourceMetadata =
            parameterInfo.GetCustomAttributes().OfType<IBindingSourceMetadata>().FirstOrDefault();

        if (bindingSourceMetadata is null)
        {
            return new ParameterBinding
            {
                BinderModelName = parameterInfo.Name,
            };
        }

        var bindingInfo = new ParameterBinding
        {
            BinderModelName = GetBinderModelNameFromMetadata(parameterInfo, bindingSourceMetadata),
            BindingSource = GetBindingSourceFromMetadata(bindingSourceMetadata)
        };

        return bindingInfo;
    }

    private static string? GetBinderModelNameFromMetadata(ParameterInfo parameterInfo,
        IBindingSourceMetadata? bindingSourceMetadata)
    {
        var binderModelName = bindingSourceMetadata switch
        {
            IFromRouteMetadata fromRouteMetadata => fromRouteMetadata.Name,
            IFromQueryMetadata fromQueryMetadata => fromQueryMetadata.Name,
            IFromHeaderMetadata fromHeaderMetadata => fromHeaderMetadata.Name,
            IFromFormMetadata fromFormMetadata => fromFormMetadata.Name,
            _ => parameterInfo.Name
        };
        return binderModelName;
    }

    private static BindingSource? GetBindingSourceFromMetadata(IBindingSourceMetadata? bindingSourceMetadata)
    {
        var bindingSource = bindingSourceMetadata switch
        {
            IFromRouteMetadata => BindingSource.Path,
            IFromQueryMetadata => BindingSource.Query,
            IFromHeaderMetadata => BindingSource.Header,
            IFromBodyMetadata => BindingSource.Body,
            IFromFormMetadata => BindingSource.Form,
            _ => null
        };
        return bindingSource;
    }

    private static Type? RecursivelyFindComplexOrSimpleType(Type? type)
    {
        var returnType = type;
        if (returnType is null or { IsGenericType: false })
        {
            return returnType;
        }

        var genericReturnType = returnType.GetGenericTypeDefinition();
        if (!returnType.IsGenericType || GenericTypesToRecurse.All(genericTypeToRecurse => genericTypeToRecurse != genericReturnType))
        {
            return returnType;
        }

        var returnTypeArgument = returnType.GenericTypeArguments.Length == 1
            ? returnType.GenericTypeArguments[0]
            : returnType.GenericTypeArguments.FirstOrDefault(typeArgument =>
                typeArgument.IsGenericType && typeArgument.GetGenericTypeDefinition() == typeof(Ok<>));

        returnType = RecursivelyFindComplexOrSimpleType(returnTypeArgument);
        return returnType;
    }

    private static readonly Type[] NonSupportedNonGenericTypes = new[]
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

    private static readonly Type[] GenericTypesToRecurse = new[]
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

    private static readonly Type[] DefaultMinimalApiInjectedTypes = new[]
    {
        typeof(HttpContext),
        typeof(HttpRequest),
        typeof(HttpResponse),
        typeof(CancellationToken),
        typeof(ClaimsPrincipal)
    };

    public string GetHttpMethod(RouteEndpoint routeEndpoint)
    {
        var httpMethodMetadata = routeEndpoint.Metadata.GetMetadata<IHttpMethodMetadata>();
        return httpMethodMetadata?.HttpMethods.FirstOrDefault() ?? "GET";
    }
}

internal record ParameterBinding
{
    public string? BinderModelName { get; init; }
    public BindingSource? BindingSource { get; init; }
}