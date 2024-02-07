using System.Reflection;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;

namespace Fydhia.Core.Common;

public record HyperMediaLink
{
    public HyperMediaLink(HttpRequest httpRequest, string path, TypeInfo? returnedType, IEnumerable<RequestParameterDescriptor>? parameters, string httpMethod, bool? templated = false)
    {
        ReturnedType = returnedType;
        Parameters = parameters;
        Href = $"{httpRequest.Scheme}://{httpRequest.Host}{path}";
        Templated = templated;
        HttpMethod = httpMethod;
    }

    public readonly TypeInfo? ReturnedType;
    public readonly IEnumerable<RequestParameterDescriptor>? Parameters;
    public readonly bool? Templated;
    public readonly string HttpMethod;
    public readonly string Href;
};