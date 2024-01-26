using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fydhia.Library;

public abstract class LinkConfiguration
{
    protected readonly IDictionary<string, string> ParameterMappings;
    protected readonly HttpVerb Verb;
    protected readonly IEnumerable<ParsedParameter> ParsedParameters;
    protected readonly ReturnedType ReturnedType;
    protected readonly Resource Group;

    public string Rel { get; private set; }
    public string? Title { get; init; }
    public string? Name { get; init; }
    public bool? Templated { get; init; }

    protected LinkConfiguration(string rel, HttpVerb verb, ReturnedType returnedType, Resource group, IEnumerable<ParsedParameter> parsedParameters, IDictionary<string, string>? parameterMappings = null)
    {
        Verb = verb;
        Group = group;
        ParsedParameters = parsedParameters;
        Rel = rel;
        ReturnedType = returnedType;
        ParameterMappings = parameterMappings ?? new Dictionary<string, string>();
    }

    public abstract HyperMediaLink GenerateHyperMediaLink(LinkFormatter linkGenerator,
        IDictionary<string, object?> responseObjectProperties);

    public void ValidateParameterMappings()
    {
        if (ParsedParameters.Count(p =>
                p.BindingSource == BindingSource.Path
                || p.BindingSource == BindingSource.Query
                || p.BindingSource == BindingSource.Header) != ParameterMappings.Count)
        {
            throw new InvalidOperationException(
                $"Parameter mappings for link configuration {Rel} are invalid. Parameter mappings must be provided for all parameters in the method signature that are bound to the path, query or header.");
        }
    }
}

public class LinkConfiguration<TControllerType>:LinkConfiguration
    where TControllerType : Controller
{
    private readonly string _controllerName;
    private readonly string _methodName;

    private LinkConfiguration(string rel, ControllerEndpoint endpointResource,
        IDictionary<string, string>? parameterMappings = null) : base(rel, endpointResource.Verb, endpointResource.Result, endpointResource.Group, endpointResource.Parameters, parameterMappings)
    {
        _controllerName = endpointResource.ControllerName;
        _methodName = endpointResource.MethodName;
    }

    internal static LinkConfiguration<TControllerType> Create(string methodName,
        IDictionary<string, string> parameterMappings, string? rel = null)
    {
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException(
                $"Method name must be provided to build a link configuration for controller {typeof(TControllerType).FullName}");
        }

        var controllerParser = new ControllerParser<TControllerType>();
        var controllerEndpoints = controllerParser.ParseEndpoints(methodName).ToList();
        if (controllerEndpoints is null || !controllerEndpoints.Any())
            throw new InvalidOperationException(
                $"Method {methodName} not found in controller {typeof(TControllerType).FullName}");

        if (controllerEndpoints.Count() > 1)
        {
            throw new InvalidOperationException(
                $"Method {methodName} in controller {typeof(TControllerType).FullName} can only have one HttpMethod to avoid ambiguity when generating links");
        }

        var controllerEndpoint = controllerEndpoints.First();
        return new LinkConfiguration<TControllerType>((rel ?? controllerEndpoint.MethodName).ToSnakeCase(), controllerEndpoint,
            parameterMappings);
    }

    public override HyperMediaLink GenerateHyperMediaLink(LinkFormatter linkGenerator, IDictionary<string, object?> responseObjectProperties)
    {
        var routeValues = BuildActionRouteValues(responseObjectProperties);
        var path = linkGenerator.FormatActionLink(_methodName, _controllerName, routeValues);
        return new HyperMediaLink(path, Verb.ToString(), ReturnedType, ParsedParameters);
    }

    private ExpandoObject BuildActionRouteValues(IDictionary<string, object?> responseObjectProperties)
    {
        var result = new ExpandoObject();
        foreach (var parameterMapping in ParameterMappings)
        {
            if (!responseObjectProperties.TryGetValue(parameterMapping.Value, out var value))
            {
                continue;
            }

            result.TryAdd(parameterMapping.Key, value);
        }

        return result;
    }
}