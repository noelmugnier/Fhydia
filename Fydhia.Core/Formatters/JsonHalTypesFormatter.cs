﻿using System.Dynamic;
using Fydhia.Core.Common;
using Fydhia.Core.Configurations;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Formatters;

public interface IHyperMediaTypesFormatter
{
    public string FormattedMediaType { get; }
    public void Format(ExpandoObject responseObject, HttpContext httpContext);
}

public class JsonHalTypesFormatter : IHyperMediaTypesFormatter
{
    private readonly LinkGenerator _linkGenerator;
    private readonly TypeConfigurationCollection _typeConfigurationCollection;

    public JsonHalTypesFormatter(TypeConfigurationCollection typeConfigurationCollection, LinkGenerator linkGenerator)
    {
        _typeConfigurationCollection = typeConfigurationCollection;
        _linkGenerator = linkGenerator;
    }

    public const string MediaType = "application/hal+json";

    public string FormattedMediaType => MediaType;

    public void Format(ExpandoObject responseObject, HttpContext httpContext)
    {
        foreach (var responseProperty in responseObject.Where(prop => prop.Value is not null && prop.Value is ExpandoObject).ToList())
        {
            Format((ExpandoObject)responseProperty.Value!, httpContext);
        }

        var typeConfiguration = _typeConfigurationCollection.GetConfiguration(responseObject.GetOriginalType());
        if (typeConfiguration is null)
            return;

        var responseObjectProperties = responseObject.ToDictionary();
        var links = new Dictionary<string, HyperMediaHalLink>();

        foreach (var linkConfiguration in typeConfiguration.ConfiguredLinks)
        {
            var href = linkConfiguration.GenerateHref(httpContext, _linkGenerator, responseObjectProperties);
            var headers = linkConfiguration.GenerateHeaders(httpContext, responseObjectProperties);
            links.TryAdd(linkConfiguration.Rel, new HyperMediaHalLink(href, linkConfiguration.Templated,
                linkConfiguration.Title, linkConfiguration.Name, headers));
        }

        responseObject.TryAdd("_links", links);
        responseObject.RemoveTypeProperty();
    }
}