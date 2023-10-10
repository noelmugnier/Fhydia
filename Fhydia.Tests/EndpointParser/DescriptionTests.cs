using System.ComponentModel;
using Fydhia.Library;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class EndpointParserDescriptionTests
{
    [Fact]
    public void ShouldParseGroupDescriptionFromControllerDescriptionAttribute()
    {
        var parsedOperation = new ControllerParser<DescriptionController>()
            .ParseEndpoints(nameof(DescriptionController.EmptyMethod))
            .First();
        Check.That(parsedOperation.Group.Description).IsEqualTo("Controller description");
    }

    [Fact]
    public void ShouldParseEndpointDescriptionFromMethodDescriptionAttribute()
    {
            var parsedOperation = new ControllerParser<DescriptionController>()
                .ParseEndpoints(
                    nameof(DescriptionController.MethodWithDescription)).First();
            Check.That(parsedOperation.Description).IsEqualTo("Method description");
    }

    [Fact]
    public void ShouldParseParamDescriptionFromParameterDescriptionAttribute()
    {
        var parsedParameter = new ControllerParser<DescriptionController>()
            .ParseEndpoints(nameof(DescriptionController
                .MethodWithParamDescription)).First().Parameters.First();
        Check.That(parsedParameter.Description).IsEqualTo("Param description");
    }

    [Fact]
    public void ShouldParseReturnTypeDescriptionFromClassDescriptionAttribute()
    {
        var parsedResult = new ControllerParser<DescriptionController>()
            .ParseEndpoints(nameof(DescriptionController
                .MethodWithReturnTypeDescription)).First().Result;
        Check.That(parsedResult.Description).IsEqualTo("Return type description");
    }

    [Fact]
    public void ShouldParsePropertyDescriptionFromPropertyDescriptionAttribute()
    {
        var parsedProperty = new ControllerParser<DescriptionController>()
            .ParseEndpoints(nameof(DescriptionController
                .MethodWithReturnTypeDescription)).First().Result.Properties.First();
        Check.That(parsedProperty.Description).IsEqualTo("Property description");
    }
}

[Description("Controller description")]
internal class DescriptionController : Controller
{
    public void EmptyMethod()
    {
    }

    [Description("Method description")]
    public void MethodWithDescription()
    {
    }

    public void MethodWithParamDescription([Description("Param description")] string param1)
    {
    }

    public ReturnTypeWithDescription MethodWithReturnTypeDescription() => new();
}

[Description("Return type description")]
public class ReturnTypeWithDescription
{
    [Description("Property description")] public string MyPropertyWithDescription { get; set; }
}