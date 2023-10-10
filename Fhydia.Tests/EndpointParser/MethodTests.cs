using Fydhia.Library;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class ControllerParserEndpointMethodTests
{
    [Fact]
    public void ShouldReturnDefaultGetVerbWhenNoVerbDefined()
    {
        var parsedOperation = new ControllerParser<VerbController>().ParseEndpoints(nameof(VerbController.MethodWithoutVerb)).First();
        Check.That(parsedOperation.Verb).IsEqualTo(HttpVerb.GET);
    }

    [Fact]
    public void ShouldReturnHttpAttributeMethodVerb()
    {
        var parsedOperation = new ControllerParser<VerbController>().ParseEndpoints(nameof(VerbController.MethodWithHttpGetVerb)).First();
        Check.That(parsedOperation.Verb).IsEqualTo(HttpVerb.POST);
    }
}

internal class VerbController : Controller
{
    public void MethodWithoutVerb() { }

    [HttpPost]
    public void MethodWithHttpGetVerb() { }
}