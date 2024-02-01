using System.ComponentModel;
using Fhydia.ControllerActions.Resources;
using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests.EndpointParser;

public class EndpointParserDescriptionTests
{
    [Fact]
    public void ShouldParseGroupDescriptionFromControllerDescriptionAttribute()
    {
        var parsedOperation = new ControllerEndpointsParser<DescriptionController>()
            .Parse(nameof(DescriptionController.EmptyMethod))
            .First<EndpointResource>();
        Check.That<string>(((ControllerEndpoint)parsedOperation).Group.Description).IsEqualTo("Controller description");
    }

    [Fact]
    public void ShouldParseEndpointDescriptionFromMethodDescriptionAttribute()
    {
            var parsedOperation = new ControllerEndpointsParser<DescriptionController>()
                .Parse(
                    nameof(DescriptionController.MethodWithDescription)).First<EndpointResource>();
            Check.That(parsedOperation.Description).IsEqualTo("Method description");
    }

    [Fact]
    public void ShouldParseParamDescriptionFromParameterDescriptionAttribute()
    {
        var parsedParameter = new ControllerEndpointsParser<DescriptionController>()
            .Parse(nameof(DescriptionController
                .MethodWithParamDescription)).First<EndpointResource>().Parameters.First();
        Check.That(parsedParameter.Description).IsEqualTo("Param description");
    }

    [Fact]
    public void ShouldParseReturnTypeDescriptionFromClassDescriptionAttribute()
    {
        var parsedResult = new ControllerEndpointsParser<DescriptionController>()
            .Parse(nameof(DescriptionController
                .MethodWithReturnTypeDescription)).First<EndpointResource>().Result;
        Check.That(parsedResult.Description).IsEqualTo("Return type description");
    }

    [Fact]
    public void ShouldParsePropertyDescriptionFromPropertyDescriptionAttribute()
    {
        var parsedProperty = new ControllerEndpointsParser<DescriptionController>()
            .Parse(nameof(DescriptionController
                .MethodWithReturnTypeDescription)).First<EndpointResource>().Result.Properties.First();
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