using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class TypeEnricher
{
    private readonly LinkGenerator _linkGenerator;

    internal TypeEnricher(LinkGenerator linkGenerator, TypeInfo typeToEnrich,
        IEnumerable<LinkConfiguration> linksConfiguration)
    {
        _linkGenerator = linkGenerator;
        TypeToEnrich = typeToEnrich;
        LinksConfiguration = linksConfiguration;
    }

    public TypeInfo TypeToEnrich { get; }
    public IEnumerable<LinkConfiguration> LinksConfiguration { get; }

    public ExpandoObject Enrich(HttpContext httpContext, ExpandoObject resultObject)
    {
        var resultProperties = resultObject.ToDictionary();
        var links = new ExpandoObject();

        foreach (var link in LinksConfiguration)
        {
            var hyperMediaLink = link.GenerateHyperMediaLink(_linkGenerator, httpContext, resultProperties);
            links.TryAdd(link.Rel, hyperMediaLink);
        }

        resultObject.TryAdd("_links", links);
        return resultObject;
    }
}