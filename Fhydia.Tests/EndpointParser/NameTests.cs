using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class EndpointParserNameTests
{
    [Fact]
    public void ShouldReturnMethodNameWhenNoAttributeUsed()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<NamingController>(nameof(NamingController.MethodWithoutName)).First();
        Check.That(parsedOperation.Name).IsEqualTo(nameof(NamingController).Replace("Controller", string.Empty) + "_" + nameof(NamingController.MethodWithoutName));
    }

    [Fact]
    public void ShouldReturnNameFromHttpAttributeNameValue()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<NamingController>(nameof(NamingController.MethodWithHttpAttributeName)).First();
        Check.That(parsedOperation.Name).IsEqualTo("http-name");
    }

    [Fact]
    public void ShouldReturnNameFromRouteAttributeNameValue()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<NamingController>(nameof(NamingController.MethodWithRouteAttributeName)).First();
        Check.That(parsedOperation.Name).IsEqualTo("route-name");
    }

    [Fact]
    public void ShouldReturnNameFromActionNameAttributeValue()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<NamingController>(nameof(NamingController.MethodWithActionNameAttribute)).First();
        Check.That(parsedOperation.Name).IsEqualTo("action-name");
    }
}

internal class NamingController : Controller
{
    public void MethodWithoutName() { }

    [HttpGet(Name = "http-name")]
    public void MethodWithHttpAttributeName() { }

    [Route("", Name = "route-name")]
    public void MethodWithRouteAttributeName() { }

    [ActionName("action-name")]
    public void MethodWithActionNameAttribute() { }
}

