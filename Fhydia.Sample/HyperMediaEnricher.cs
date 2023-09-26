using System.Dynamic;
using Microsoft.AspNetCore.Mvc;

namespace Fhydia.Sample;

public class HyperMediaEnricher
{
    private readonly HyperMediaConfiguration _config;

    public HyperMediaEnricher(HyperMediaConfiguration config)
    {
        _config = config;
    }

    public IActionResult EnrichResult(IActionResult result)
    {
        if (result is not ObjectResult objectResult)
            return result;

        var expando = objectResult.Value.ToExpando();

        _config.GetTypeConfigs(objectResult.Value.GetType()).ForEach(config => config.Enrich(expando));

        objectResult.Value = expando;

        return result;
    }
}

public class HyperMediaConfiguration
{
    public HyperMediaConfiguration(List<TypeConfig> configs)
    {
        Configs = configs;
    }

    public List<TypeConfig> Configs { get; }

    public List<TypeConfig> GetTypeConfigs(Type type)
    {
        return Configs.Where(c => c.Type == type).ToList();
    }
}

public class TypeConfig
{
    private readonly IDictionary<string, Link> Links = new Dictionary<string, Link>();

    public TypeConfig(Type type)
    {
        Type = type;
    }

    public Type Type { get; }

    public void AddLink(string rel, string href, HttpVerb operation = HttpVerb.GET)
    {
        Links.Add(rel.ToSnakeCase(), new Link(href, new HttpMethod(operation.ToString())));
    }

    public void Enrich(ExpandoObject expando)
    {
        expando.TryAdd("_links", Links);
    }
}

public class Link
{
    public Link(string href, HttpMethod method)
    {
        Href = href;
        Method = method;
    }

    public string Href { get; set; }
    public HttpMethod Method { get; set; }
}

public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE,
    PATCH
}
