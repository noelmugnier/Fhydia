using System.Reflection;

namespace Fydhia.Library;

public abstract class EndpointResource : Resource
{
    public HttpVerb Verb { get; }
    public IEnumerable<ParsedParameter> Parameters { get; }
    public ReturnedType Result { get; }
    public ControllerGroup Group { get; }

    public EndpointResource(
        string name,
        TypeInfo relatedType,
        HttpVerb verb,
        ControllerGroup group,
        ReturnedType result,
        IEnumerable<ParsedParameter> parameters) : base(name, relatedType)
    {
        Verb = verb;
        Group = group;
        Result = result;
        Parameters = parameters;
    }
}