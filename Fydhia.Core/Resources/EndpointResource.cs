using System.Reflection;
using Fydhia.Core.Common;

namespace Fydhia.Core.Resources;

public abstract class EndpointResource : Resource
{
    public HttpVerb Verb { get; }
    public IEnumerable<ParsedParameter> Parameters { get; }
    public ReturnedType Result { get; }

    public EndpointResource(
        string name,
        TypeInfo relatedType,
        HttpVerb verb,
        ReturnedType result,
        IEnumerable<ParsedParameter> parameters) : base(name, relatedType)
    {
        Verb = verb;
        Result = result;
        Parameters = parameters;
    }
}