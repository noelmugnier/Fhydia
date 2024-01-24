using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public abstract class EndpointResource : Resource
{
    public HttpVerb Verb { get; }
    public IEnumerable<ParsedParameter> Parameters { get; }
    public ControllerEndpointResult Result { get; }
    public ControllerGroup Group { get; }

    public EndpointResource(
        string name,
        TypeInfo relatedType,
        HttpVerb verb,
        ControllerGroup group,
        ControllerEndpointResult result,
        IEnumerable<ParsedParameter> parameters) : base(name, relatedType)
    {
        Verb = verb;
        Group = group;
        Result = result;
        Parameters = parameters;
    }

    public abstract HyperMediaLink GenerateHyperMediaLink(LinkGenerator linkGenerator,
        IDictionary<string, string> parametersMapping,
        HttpContext httpContext, IDictionary<string, object?> values);
}