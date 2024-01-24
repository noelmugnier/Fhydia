using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class LinkConfiguration
{
    private IDictionary<string, string> _parameterMappings;
    private EndpointResource _endpoint;

    public string Rel { get; private set; }
    public string UniqueIdentifier => _endpoint.Name;
    public IEnumerable<ParsedParameter> Parameters => _endpoint.Parameters;

    private LinkConfiguration(string rel, EndpointResource endpointResource,
        IDictionary<string, string> parameterMappings)
    {
        Rel = rel;
        _endpoint = endpointResource;
        _parameterMappings = parameterMappings;
    }

    internal static LinkConfiguration CreateForController<TControllerType>(string methodName,
        IDictionary<string, string> parameterMappings, string? rel = null)
        where TControllerType : Controller
    {
        var controllerParser = new ControllerParser<TControllerType>();
        var controllerEndpoints = controllerParser.ParseEndpoints(methodName).ToList();
        if (controllerEndpoints is null || !controllerEndpoints.Any())
            throw new InvalidOperationException(
                $"Method {methodName} not found in controller {typeof(TControllerType).FullName}");

        if (controllerEndpoints.Count() > 1)
            throw new InvalidOperationException(
                $"Method {methodName} in controller {typeof(TControllerType).FullName} can only have one HttpMethod to avoid ambiguity when generating links");

        var controllerEndpoint = controllerEndpoints.First();
        return new LinkConfiguration((rel ?? controllerEndpoint.MethodName).ToSnakeCase(), controllerEndpoint,
            parameterMappings);
    }

    public HyperMediaLink GenerateHyperMediaLink(LinkGenerator linkGenerator, HttpContext httpContext,
        IDictionary<string, object> resultProperties)
    {
        return _endpoint.GenerateHyperMediaLink(linkGenerator, _parameterMappings, httpContext, resultProperties);
    }
}