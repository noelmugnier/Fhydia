using Microsoft.AspNetCore.Http;

namespace Fydhia.Core.Common;

public record HyperMediaLink
{
    public HyperMediaLink(HttpRequest httpRequest, string path, bool templated = false)
    {
        Href = $"{httpRequest.Scheme}://{httpRequest.Host}{path}";
        Templated = templated;
    }

    public readonly bool Templated;

    public readonly string Href;
};