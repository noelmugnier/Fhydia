using System.ComponentModel;
using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorDescriptionTests
{
    [Fact]
    public void ShouldParseGroupDescriptionFromController()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<DescriptionController>(nameof(DescriptionController.EmptyMethod)).First();
        Check.That(parsedOperation.Group.Description).IsEqualTo("Controller description");
    }

    [Fact]
    public void ShouldParseEndpointDescription()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<DescriptionController>(nameof(DescriptionController.MethodWithDescription)).First();
        Check.That(parsedOperation.Description).IsEqualTo("Method description");
    }

    [Fact]
    public void ShouldParseParamDescription()
    {
        var parsedParameter = new EndpointParser().ParseControllerOperationEndpoints<DescriptionController>(nameof(DescriptionController.MethodWithParamDescription)).First().Parameters.First();
        Check.That(parsedParameter.Description).IsEqualTo("Param description");
    }

    [Fact]
    public void ShouldParseReturnTypeDescription()
    {
        var parsedResult = new EndpointParser().ParseControllerOperationEndpoints<DescriptionController>(nameof(DescriptionController.MethodWithReturnTypeDescription)).First().Result;
        Check.That(parsedResult.Description).IsEqualTo("Return type description");
    }

    [Fact]
    public void ShouldParsePropertyDescription()
    {
        var parsedProperty = new EndpointParser().ParseControllerOperationEndpoints<DescriptionController>(nameof(DescriptionController.MethodWithReturnTypeDescription)).First().Result.Properties.First();
        Check.That(parsedProperty.Description).IsEqualTo("Property description");
    }
}

[Description("Controller description")]
internal class DescriptionController : Controller
{
    public void EmptyMethod() { }

    [Description("Method description")]
    public void MethodWithDescription() { }

    public void MethodWithParamDescription([Description("Param description")] string param1) { }

    public ReturnTypeWithDescription MethodWithReturnTypeDescription() => new();
}

[Description("Return type description")]
public class ReturnTypeWithDescription
{
    [Description("Property description")]
    public string MyPropertyWithDescription { get; set; }
}
