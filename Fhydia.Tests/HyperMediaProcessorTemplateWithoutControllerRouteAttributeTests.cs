using System.Reflection;
using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorTemplateWithoutControllerRouteAttributeTests
{
    [Fact]
    public void ShouldReturnControllerNameConcatenatedWithMethodNameWhenNoAttribute()
    {
        var processor = new Processor();
        var parsedOperations = processor.ParseController(typeof(WithoutRouteController).GetTypeInfo());
        var operation = parsedOperations.FirstOrDefault(c => c.MethodInfo.Name == nameof(WithoutRouteController.MethodWithoutAttributes));

        Check.That(operation.Template.ToString()).IsEqualTo("WithoutRoute/MethodWithoutAttributes");
    }

    [Fact]
    public void ShouldReturnControllerNameConcatenatedWithMethodNameWhenUsingHttpAttributeWithNoTemplate()
    {
        var processor = new Processor();
        var parsedOperations = processor.ParseController(typeof(WithoutRouteController).GetTypeInfo());
        var operation = parsedOperations.FirstOrDefault(c => c.MethodInfo.Name == nameof(WithoutRouteController.MethodWithHttpAttribute));

        Check.That(operation.Template.ToString()).IsEqualTo("WithoutRoute/MethodWithHttpAttribute");
    }

    [Fact]
    public void ShouldReturnEmptyRouteWhenUsingHttpAttributeWithEmptyTemplate()
    {
        var processor = new Processor();
        var parsedOperations = processor.ParseController(typeof(WithoutRouteController).GetTypeInfo());
        var operation = parsedOperations.FirstOrDefault(c => c.MethodInfo.Name == nameof(WithoutRouteController.MethodWithHttpAttributeEmpty));

        Check.That(operation.Template.ToString()).IsEqualTo("");
    }

    [Fact]
    public void ShouldReturnNonEmptyRouteWhenUsingHttpAttributeWithTemplate()
    {
        var processor = new Processor();
        var parsedOperations = processor.ParseController(typeof(WithoutRouteController).GetTypeInfo());
        var operation = parsedOperations.FirstOrDefault(c => c.MethodInfo.Name == nameof(WithoutRouteController.MethodWithHttpAttributeNonEmpty));

        Check.That(operation.Template.ToString()).IsEqualTo("HttpRouteNonEmpty");
    }

    [Fact]
    public void ShouldThrowExceptionWhenHttpAttributeTemplateAndRouteTemplateUsedOnSameMethod()
    {
        var processor = new Processor();
        Assert.Throws<InvalidOperationException>(() => processor.ParseController(typeof(ErrorController).GetTypeInfo()));
    }
}

internal class WithoutRouteController : Controller
{
    public void MethodWithoutAttributes() { }

    [HttpGet]
    public void MethodWithHttpAttribute() { }

    [HttpGet("")]
    public void MethodWithHttpAttributeEmpty() { }

    [HttpGet("HttpRouteNonEmpty")]
    public void MethodWithHttpAttributeNonEmpty() { }
}

internal class ErrorController : Controller
{
    [HttpGet("")]
    [Route("RouteAttributeEmpty")]
    public void MethodWithHttpAttributeEmptyAndRouteAttribute() { }
}
