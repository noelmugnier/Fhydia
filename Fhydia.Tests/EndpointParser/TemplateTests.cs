using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class EndpointParserTemplateTests
{
    [Fact]
    public void ShouldReturnControllerTypeAsTypeInfoForEndpointType()
    {
        var parsedOperation = new EndpointParser().ParseControllerEndpoints<WithoutRouteController>().First();
        Check.That(parsedOperation.TypeInfo.AsType()).IsEqualTo(typeof(WithoutRouteController));
    }

    [Fact]
    public void ShouldNotParseControllerWithNonControllerAttribute()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<NonControllerAttributeController>();
        Check.That(parsedOperations).HasSize(0);
    }

    [Fact]
    public void ShouldNotParseControllerMethodsWithNonActionAtrribute()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<ControllerWithNonActionAttributeController>();
        Check.That(parsedOperations).HasSize(1);
        Check.That(parsedOperations.First().MethodInfo.Name).IsEqualTo(nameof(ControllerWithNonActionAttributeController.MethodWithoutAttributes));
    }

    [Fact]
    public void ShouldParseMethodInChildAndNonAbstractParent()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<ChildWithSimpleParentController>();
        Check.That(parsedOperations).HasSize(2);
    }

    [Fact]
    public void ShouldParseMethodInChildAndAbstractParent()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<ChildWithAbstractParentController>();
        Check.That(parsedOperations).HasSize(2);
    }

    [Fact]
    public void ShouldNotParseMethodDirectlyFromAbstractController()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<AbstractParentController>();
        Check.That(parsedOperations).HasSize(0);
    }

    [Fact]
    public void ShouldParseMethodInChildWithChildControllerName()
    {
        var parsedOperations = new EndpointParser().ParseControllerOperationEndpoints<ChildWithSimpleParentController>(nameof(ChildWithSimpleParentController.MethodInChild)).First();
        Check.That(parsedOperations.Template.ToString()).IsEqualTo("ChildWithSimpleParent/MethodInChild");
    }

    [Fact]
    public void ShouldParseMethodInParentWithChildControllerName()
    {
        var parsedOperations = new EndpointParser().ParseControllerOperationEndpoints<ChildWithSimpleParentController>(nameof(ChildWithSimpleParentController.MethodInSimpleParent)).First();
        Check.That(parsedOperations.Template.ToString()).IsEqualTo("ChildWithSimpleParent/MethodInSimpleParent");
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
    public void ShouldThrowExceptionWhenHttpAttributeTemplateAndRouteTemplateUsedOnSameMethod()
    {
        Assert.Throws<InvalidOperationException>(() => new EndpointParser().ParseControllerEndpoints<ErrorController>());
    }

    [Fact]
    public void ShouldThrowExceptionWhenAttributedControllerHaveNonAttributedMethods()
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

    [Fact]
    public void ShouldReturnMappingWithControllerNameUsingPlaceholder()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithRoutePlaceholderController>(nameof(WithRoutePlaceholderController.MethodWithoutPlaceholderInAttribute)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("api/WithRoutePlaceholder/Testing");
    }

    [Fact]
    public void ShouldReturnMappingWithActionNameUsingPlaceholder()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithRoutePlaceholderController>(nameof(WithRoutePlaceholderController.MethodWithPlaceholderInAttribute)).First();
        Check.That(parsedOperation.Template.ToString()).IsEqualTo("api/WithRoutePlaceholder/test/MethodWithPlaceholderInAttribute");
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

internal abstract class AbstractParentController : Controller
{
    public void MethodInAbstractParent() { }
}

internal class SimpleParentController : Controller
{
    public void MethodInSimpleParent() { }
}

internal class ChildWithSimpleParentController : SimpleParentController
{
    public void MethodInChild() { }
}

internal class ChildWithAbstractParentController : AbstractParentController
{
    public void MethodInChild() { }
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

[Route("api/[controller]")]
internal class WithRoutePlaceholderController : Controller
{
    [HttpGet("test/[action]")]
    public void MethodWithPlaceholderInAttribute() { }

    [HttpGet("Testing")]
    public void MethodWithoutPlaceholderInAttribute() { }
}

[NonController]
internal class NonControllerAttributeController : Controller
{
    public void Method() { }
}

internal class ControllerWithNonActionAttributeController : Controller
{
    public void MethodWithoutAttributes() { }

    [NonAction]
    public void MethodWithNonActionAttribute() { }
}


