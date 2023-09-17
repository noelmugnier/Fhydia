using System.Reflection;
using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorTemplateControllerWithRouteAttributeTests
{
    [Fact]
    public void ShouldThrowExceptionForMethodWithNoAttributes()
    {
        var processor = new Processor();
        Assert.Throws<InvalidOperationException>(() => processor.ParseController(typeof(WithRouteErrorController).GetTypeInfo()));
    }

    [Fact]
    public void ShouldReturnMappingWithControllerRouteAndMethodFromHttpAttribute()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(WithRouteController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(WithRouteController.MethodWithHttpAttributeNonEmpty));
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("Test/HttpRouteOnly");
    }

    [Fact]
    public void ShouldReturnMappingWithMethodHttpAttributeTemplateOnly()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(WithRouteController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(WithRouteController.MethodWithHttpAttributeStartingWithSlash));
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("RootUrl");
    }

    [Fact]
    public void ShouldReturnMappingWithMethodRouteAttributeTemplateOnly()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(WithRouteController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(WithRouteController.MethodWithRouteAttributeStartingWithSlash));
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
