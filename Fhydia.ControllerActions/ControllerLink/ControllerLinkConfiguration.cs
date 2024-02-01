using Fhydia.ControllerActions.Parsers;
using Fhydia.ControllerActions.Resources;
using Fydhia.Core.Common;
using Fydhia.Core.Configurations;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fhydia.ControllerActions.ControllerLink;

public class ControllerLinkConfiguration<TControllerType> : LinkConfiguration
    where TControllerType : Controller
{
    private readonly string _controllerName;
    private readonly string _methodName;

    private ControllerLinkConfiguration(string rel, ControllerEndpoint endpointResource,
        IDictionary<string, string>? parameterMappings = null) : base(rel, endpointResource.Verb,
        endpointResource.Result, endpointResource.Group, endpointResource.Parameters, parameterMappings)
    {
        _controllerName = endpointResource.ControllerName;
        _methodName = endpointResource.MethodName;
    }

    internal static ControllerLinkConfiguration<TControllerType> Create(string methodName,
        IDictionary<string, string> parameterMappings, string? rel = null)
    {
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException(
                $"Method name must be provided to build a link configuration for controller {typeof(TControllerType).FullName}");
        }

        var controllerParser = new ControllerEndpointsParser<TControllerType>();
        var controllerEndpoints = controllerParser.Parse(methodName).ToList();
        if (controllerEndpoints is null || !controllerEndpoints.Any())
            throw new InvalidOperationException(
                $"Method {methodName} not found in controller {typeof(TControllerType).FullName}");

        if (controllerEndpoints.Count() > 1)
        {
            throw new InvalidOperationException(
                $"Method {methodName} in controller {typeof(TControllerType).FullName} can only have one HttpMethod to avoid ambiguity when generating links");
        }

        var controllerEndpoint = (ControllerEndpoint)controllerEndpoints.First();
        return new ControllerLinkConfiguration<TControllerType>((rel ?? controllerEndpoint.MethodName).ToSnakeCase(),
            controllerEndpoint,
            parameterMappings);
    }

    public override HyperMediaLink GenerateHyperMediaLink(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        var routeValues = BuildActionRouteValues(responseObjectProperties);
        var path = linkGenerator.GetUriByAction(httpContext, _methodName, _controllerName, routeValues,
            options: new LinkOptions { LowercaseUrls = true, LowercaseQueryStrings = true });

        return new HyperMediaLink(path, Verb.ToString(), ReturnedType, ParsedParameters);
    }
}