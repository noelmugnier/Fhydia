using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class JsonHalTypeFormatter : HypermediaTypeFormatter
{
    private readonly HttpContext _httpContext;
    private readonly LinkGenerator _linkGenerator;
    public string MediaType => "application/hal+json";

    public JsonHalTypeFormatter(HttpContext httpContext, LinkGenerator linkGenerator)
    {
        _httpContext = httpContext;
        _linkGenerator = linkGenerator;
    }

    public override ExpandoObject Format(ExpandoObject value, TypeEnricherConfiguration typeEnricherConfiguration)
    {
        var properties = value.ToDictionary();
        var links = new ExpandoObject();

        foreach (var linkConfiguration in typeEnricherConfiguration.ConfiguredLinks)
        {
            var hyperMediaLink = linkConfiguration.GenerateHyperMediaLink(_linkGenerator, _httpContext, properties);
            links.TryAdd(linkConfiguration.Rel, hyperMediaLink);
        }

        value.TryAdd("_links", links);
        value.RemoveTypeProperty();
        return value;
    }
}