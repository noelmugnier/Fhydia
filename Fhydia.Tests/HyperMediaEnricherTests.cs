using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaEnricherTests
{
    [Fact]
    public void ShouldAddLinksToResult()
    {
        var builder = new HyperMediaConfigurationTestBuilder().AddResultType<Model1>(out var type);
        type.AddLink("self", "http://localhost");
        var sut = new HyperMediaEnricher(builder.Build());

        var result = sut.EnrichResult(new ObjectResult(new Model1())) as ObjectResult;

        Check.That(result.GetLinks()).IsNotNull().And.HasSize(1);
    }

    [Fact]
    public void ShouldAddLinksToCorrespondingTypeOnly()
    {
        var builder = new HyperMediaConfigurationTestBuilder()
          .AddResultType<Model1>(out var _)
          .AddResultType<Model2>(out var type);

        type.AddLink("self", "http://localhost");
        type.AddLink("target", "http://localhost");

        var sut = new HyperMediaEnricher(builder.Build());

        var result = sut.EnrichResult(new ObjectResult(new Model2())) as ObjectResult;

        Check.That(result.GetLinks()).IsNotNull().And.HasSize(2);
    }

    [Fact]
    public void ShouldAddLinkWithMethod()
    {
        var builder = new HyperMediaConfigurationTestBuilder().AddResultType<Model1>(out var type);
        type.AddLink("self", "http://localhost", HttpVerb.POST);
        var sut = new HyperMediaEnricher(builder.Build());

        var result = sut.EnrichResult(new ObjectResult(new Model1())) as ObjectResult;
        var method = result.GetLinks().TryGetValue("self", out var link) ? link.Method : null;
        Check.That(method).IsNotNull().And.IsEqualTo(HttpMethod.Post);
    }

    [Fact]
    public void ShouldAddLinkWithLinkNameAsSnakeCase()
    {
        var builder = new HyperMediaConfigurationTestBuilder().AddResultType<Model1>(out var type);
        type.AddLink("GetMyProduct", "http://localhost", HttpVerb.POST);
        var sut = new HyperMediaEnricher(builder.Build());

        var result = sut.EnrichResult(new ObjectResult(new Model1())) as ObjectResult;
        var snakeCasedLink = result.GetLinks().TryGetValue("get_my_product", out var link) ? link : null;
        Check.That(snakeCasedLink).IsNotNull();
    }
}

internal class Model1
{
}

internal class Model2
{
}

internal class HyperMediaConfigurationTestBuilder
{
    public List<TypeConfig> Configs { get; private set; } = new List<TypeConfig>();

    public HyperMediaConfigurationTestBuilder()
    {
    }

    internal HyperMediaConfigurationTestBuilder AddResultType<T>(out TypeConfig typeConfig)
    {
        typeConfig = new TypeConfig(typeof(T));
        Configs.Add(typeConfig);
        return this;
    }

    internal HyperMediaConfiguration Build()
    {
        return new HyperMediaConfiguration(Configs);
    }
}

internal static class ActionResultExtensions
{
    public static IDictionary<string, Link> GetLinks(this IActionResult result)
    {
        if (result is not ObjectResult objectResult)
            return new Dictionary<string, Link>();

        var dict = objectResult.Value as IDictionary<string, object>;
        if (dict is null)
            return new Dictionary<string, Link>();

        if (dict.TryGetValue("_links", out var links))
            return (Dictionary<string, Link>)links;

        return new Dictionary<string, Link>();
    }
}
