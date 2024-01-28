using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class JsonHalTypeFormatter : HypermediaTypeFormatter
{
    private readonly LinkGenerator _linkGenerator;
    private readonly TypeConfigurationCollection _typeConfiguration;

    public const string MediaType = "application/hal+json";

    public JsonHalTypeFormatter(HyperMediaConfiguration hyperMediaConfiguration, LinkGenerator linkGenerator)
    {
        _typeConfiguration = hyperMediaConfiguration.ConfiguredTypes;
        _linkGenerator = linkGenerator;
    }

    public override void Format(ExpandoObject responseObject, HttpContext httpContext)
    {
        foreach (var responseProperty in responseObject.Where(prop => prop.Value is not null && prop.Value is ExpandoObject).ToList())
        {
            Format((ExpandoObject)responseProperty.Value!, httpContext);
        }

        var typeConfiguration = _typeConfiguration.GetConfiguration(responseObject.GetOriginalType());
        if (typeConfiguration is null)
            return;

        var responseObjectProperties = responseObject.ToDictionary();
        var links = new Dictionary<string, HyperMediaHalLink>();

        foreach (var linkConfiguration in typeConfiguration.ConfiguredLinks)
        {
            var hyperMediaLink =
                linkConfiguration.GenerateHyperMediaLink(httpContext, _linkGenerator, responseObjectProperties);
            links.TryAdd(linkConfiguration.Rel,
                new HyperMediaHalLink(hyperMediaLink.Href, linkConfiguration.Title, linkConfiguration.Name,
                    linkConfiguration.Templated));
        }

        responseObject.TryAdd("_links", links);
        responseObject.RemoveTypeProperty();
    }
}