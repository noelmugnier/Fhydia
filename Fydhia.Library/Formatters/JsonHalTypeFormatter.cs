using System.Dynamic;

namespace Fydhia.Library;

public class JsonHalTypeFormatter : HypermediaTypeFormatter
{
    private readonly LinkFormatter _linkGenerator;

    public const string MediaType = "application/hal+json";

    public JsonHalTypeFormatter(LinkFormatter linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public override ExpandoObject Format(ExpandoObject responseObject, TypeEnricherConfiguration typeEnricherConfiguration)
    {
        var responseObjectProperties = responseObject.ToDictionary();
        var links = new Dictionary<string, HyperMediaHalLink>();

        foreach (var linkConfiguration in typeEnricherConfiguration.ConfiguredLinks)
        {
            var hyperMediaLink = linkConfiguration.GenerateHyperMediaLink(_linkGenerator, responseObjectProperties);
            links.TryAdd(linkConfiguration.Rel, new HyperMediaHalLink(hyperMediaLink.Href, linkConfiguration.Title, linkConfiguration.Name, linkConfiguration.Templated));
        }

        responseObject.TryAdd("_links", links);
        responseObject.RemoveTypeProperty();

        return responseObject;
    }
}