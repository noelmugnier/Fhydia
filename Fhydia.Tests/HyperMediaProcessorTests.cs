using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorTemplateControllerWithRouteAttributeTests
{
    [Fact]
    public void ShouldThrowExceptionForMethodWithNoAttributes()
    {
        Assert.Throws<InvalidOperationException>(() => new EndpointParser().ParseControllerEndpoints<WithRouteErrorController>());
    }

    [Fact]
    public void ShouldReturnMappingWithControllerRouteAndMethodFromHttpAttribute()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithRouteController>(nameof(WithRouteController.MethodWithHttpAttributeNonEmpty)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("Test/HttpRouteOnly");
    }

    [Fact]
    public void ShouldReturnMappingWithMethodHttpAttributeTemplateOnly()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithRouteController>(nameof(WithRouteController.MethodWithHttpAttributeStartingWithSlash)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("RootUrl");
    }

    [Fact]
    public void ShouldReturnMappingWithMethodRouteAttributeTemplateOnly()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithRouteController>(nameof(WithRouteController.MethodWithRouteAttributeStartingWithSlash)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("RouteUrl");
    }
}

[Route("Test")]
internal class WithRouteErrorController : Controller
{
    public void MethodWithoutAttributes() { }
}

[Route("Test")]
internal class WithRouteController : Controller
{
    [HttpGet("HttpRouteOnly")]
    public void MethodWithHttpAttributeNonEmpty() { }

    [HttpGet("/RootUrl")]
    public void MethodWithHttpAttributeStartingWithSlash() { }

    [Route("/RouteUrl")]
    public void MethodWithRouteAttributeStartingWithSlash() { }
}
