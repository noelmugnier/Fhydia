using Fydhia.Core.Common;
using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests.EndpointParser;

public class ControllerParserEndpointMethodTests
{
    [Fact]
    public void ShouldReturnDefaultGetVerbWhenNoVerbDefined()
    {
        var parsedOperation = new ControllerEndpointsParser<VerbController>().Parse(nameof(VerbController.MethodWithoutVerb)).First<EndpointResource>();
        Check.That(parsedOperation.Verb).IsEqualTo(HttpVerb.GET);
    }

    [Fact]
    public void ShouldReturnHttpAttributeMethodVerb()
    {
        var parsedOperation = new ControllerEndpointsParser<VerbController>().Parse(nameof(VerbController.MethodWithHttpGetVerb)).First<EndpointResource>();
        Check.That(parsedOperation.Verb).IsEqualTo(HttpVerb.POST);
    }
}

internal class VerbController : Controller
{
    public void MethodWithoutVerb() { }

    [HttpPost]
    public void MethodWithHttpGetVerb() { }
}