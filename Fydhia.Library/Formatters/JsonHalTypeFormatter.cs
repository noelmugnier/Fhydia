using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class JsonHalTypeFormatter : HypermediaTypeFormatter
{
    private readonly LinkGenerator _linkGenerator;

    public const string MediaType = "application/hal+json";

    public JsonHalTypeFormatter(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public override ExpandoObject Format(ExpandoObject responseObject, TypeEnricherConfiguration typeEnricherConfiguration, HttpContext httpContext)
    {
        var responseObjectProperties = responseObject.ToDictionary();
        var links = new Dictionary<string, HyperMediaHalLink>();

        foreach (var linkConfiguration in typeEnricherConfiguration.ConfiguredLinks)
        {
            var hyperMediaLink = linkConfiguration.GenerateHyperMediaLink(httpContext, _linkGenerator, responseObjectProperties);
            links.TryAdd(linkConfiguration.Rel, new HyperMediaHalLink(hyperMediaLink.Href, linkConfiguration.Title, linkConfiguration.Name, linkConfiguration.Templated));
        }

        responseObject.TryAdd("_links", links);
        responseObject.RemoveTypeProperty();

        return responseObject;
    }
}