using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class FhydiaEnricherTests
{
    [Fact]
    public void ShouldAddLinksToConfiguredObject()
    {
        var config = new FhydiaConfigurationTestBuilder().AddResultType<Test>().Build();
        var sut = new FhydiaEnricher(config);
        var result = sut.EnrichResult(new ObjectResult(new Test())) as ObjectResult;
        Check.That(result.GetLinks()).IsNotNull().And.HasSize(1);
    }
}

internal class FhydiaConfigurationTestBuilder
{
    public FhydiaConfigurationTestBuilder()
    {
    }

    internal FhydiaConfigurationTestBuilder AddResultType<T>()
    {
        throw new NotImplementedException();
    }

    internal FhydiaConfiguration Build()
    {
        throw new NotImplementedException();
    }
}
