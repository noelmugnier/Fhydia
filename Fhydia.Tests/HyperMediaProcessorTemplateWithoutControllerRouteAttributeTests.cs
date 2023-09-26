using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorTemplateWithoutControllerRouteAttributeTests
{
    [Fact]
    public void ShouldReturnControllerNameConcatenatedWithMethodNameWhenNoAttribute()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithoutRouteController>(nameof(WithoutRouteController.MethodWithoutAttributes)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("WithoutRoute/MethodWithoutAttributes");
    }

    [Fact]
    public void ShouldReturnControllerNameConcatenatedWithMethodNameWhenUsingHttpAttributeWithNoTemplate()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithoutRouteController>(nameof(WithoutRouteController.MethodWithHttpAttribute)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("WithoutRoute/MethodWithHttpAttribute");
    }

    [Fact]
    public void ShouldReturnEmptyRouteWhenUsingHttpAttributeWithEmptyTemplate()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithoutRouteController>(nameof(WithoutRouteController.MethodWithHttpAttributeEmpty)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("");
    }

    [Fact]
    public void ShouldReturnNonEmptyRouteWhenUsingHttpAttributeWithTemplate()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithoutRouteController>(nameof(WithoutRouteController.MethodWithHttpAttributeNonEmpty)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("HttpRouteNonEmpty");
    }

    [Fact]
    public void ShouldThrowExceptionWhenHttpAttributeTemplateAndRouteTemplateUsedOnSameMethod()
    {
        Assert.Throws<InvalidOperationException>(() => new EndpointParser().ParseControllerEndpoints<ErrorController>());
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
