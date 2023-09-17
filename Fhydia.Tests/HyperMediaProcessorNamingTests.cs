using System.Reflection;
using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorNamingTests
{
    [Fact]
    public void ShouldParseOperationNameAsMethodName()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(NamingController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(NamingController.MethodWithoutName));

        Check.That(parsedOperation.Name).IsEqualTo(nameof(NamingController).Replace("Controller", string.Empty) + "_" + nameof(NamingController.MethodWithoutName));
    }

    [Fact]
    public void ShouldParseOperationNameFromHttpAttributeName()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(NamingController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(NamingController.MethodWithHttpAttributeName));

        Check.That(parsedOperation.Name).IsEqualTo("http-name");
    }

    [Fact]
    public void ShouldParseOperationNameFromRouteAttributeName()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(NamingController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(NamingController.MethodWithRouteAttributeName));

        Check.That(parsedOperation.Name).IsEqualTo("route-name");
    }

    [Fact]
    public void ShouldParseOperationNameFromActionName()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(NamingController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(NamingController.MethodWithActionNameAttribute));

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

