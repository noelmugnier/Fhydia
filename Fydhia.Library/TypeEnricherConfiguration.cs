using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Library;

public class TypeEnricherConfiguration
{
    internal TypeEnricherConfiguration(TypeInfo typeToEnrich,
        IEnumerable<LinkConfiguration> linksConfiguration)
    {
        TypeToEnrich = typeToEnrich;
        LinksConfiguration = linksConfiguration;
    }

    public TypeInfo TypeToEnrich { get; }

    public IEnumerable<LinkConfiguration> LinksConfiguration { get; }
}

public static class HypermediaTypeFormatterFactory
{
    public static IHypermediaTypeFormatter Create(HttpContext httpContext, LinkGenerator linkGenerator)
    {
        var acceptedMediaType = httpContext.Request.Headers[HeaderNames.Accept].FirstOrDefault();
        if (acceptedMediaType is null)
            throw new NotSupportedException();

        switch (acceptedMediaType)
        {
            case "application/hal+json":
                return new JsonHalTypeFormatter(httpContext, linkGenerator);
            default:
                throw new NotSupportedException();
        }
    }
}

public class JsonHalTypeFormatter : IHypermediaTypeFormatter
{
    private readonly HttpContext _httpContext;
    private readonly LinkGenerator _linkGenerator;
    public string MediaType => "application/hal+json";

    public JsonHalTypeFormatter(HttpContext httpContext, LinkGenerator linkGenerator)
    {
        _httpContext = httpContext;
        _linkGenerator = linkGenerator;
    }

    public ExpandoObject Format(ExpandoObject result, TypeEnricherConfiguration config)
    {
        var resultProperties = result.ToDictionary();
        var links = new ExpandoObject();

        foreach (var link in config.LinksConfiguration)
        {
            var hyperMediaLink = link.GenerateHyperMediaLink(_linkGenerator, _httpContext, resultProperties);
            links.TryAdd(link.Rel, hyperMediaLink);
        }

        result.TryAdd("_links", links);
        result.RemoveTypeProperty();
        return result;
    }
}

public interface IHypermediaTypeFormatter
{
    ExpandoObject Format(ExpandoObject result, TypeEnricherConfiguration config);
}