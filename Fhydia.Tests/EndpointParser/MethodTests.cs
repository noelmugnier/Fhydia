using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class EndpointParserMethodTests
{
    [Fact]
    public void ShouldReturnDefaultGetVerbWhenNoVerbDefined()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<VerbController>(nameof(VerbController.MethodWithoutVerb)).First();
        Check.That(parsedOperation.Method).IsEqualTo(HttpVerb.GET);
    }

    [Fact]
    public void ShouldReturnHttpAttributeMethodVerb()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<VerbController>(nameof(VerbController.MethodWithHttpGetVerb)).First();
        Check.That(parsedOperation.Method).IsEqualTo(HttpVerb.POST);
    }
}

internal class VerbController : Controller
{
    public void MethodWithoutVerb() { }

    [HttpPost]
    public void MethodWithHttpGetVerb() { }
}

