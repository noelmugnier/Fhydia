using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NFluent;

namespace Fhydia.Tests;

public class EndpointParserParamsTests
{
    [Fact]
    public void ShouldReturnMethodParamsAsList()
    {
        var parsedParameters = new EndpointParser().ParseControllerOperationEndpoints<ParamsController>(nameof(ParamsController.MethodWithParams)).First();
        Check.That(parsedParameters.Parameters).HasSize(2);
    }

    [Fact]
    public void ShouldReturnTypeInfoForParameter()
    {
        var parsedParam = new EndpointParser().ParseControllerOperationEndpoints<ParamsController>(nameof(ParamsController.MethodWithParamsFromQuery)).First().Parameters.First();
        Check.That(parsedParam.TypeInfo.AsType()).IsEqualTo(typeof(string));
    }

    [Fact]
    public void ShouldParseParamBindingSourceFormQueryAttribute()
    {
        var parsedParam = new EndpointParser().ParseControllerOperationEndpoints<ParamsController>(nameof(ParamsController.MethodWithParamsFromQuery)).First().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Query);
    }

    [Fact]
    public void ShouldParseParamBindingSourceFormRouteAttribute()
    {
        var parsedParam = new EndpointParser().ParseControllerOperationEndpoints<ParamsController>(nameof(ParamsController.MethodWithParamsFromRoute)).First().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Path);
    }

    [Fact]
    public void ShouldParseParamBindingSourceFormBodyAttribute()
    {
        var parsedParam = new EndpointParser().ParseControllerOperationEndpoints<ParamsController>(nameof(ParamsController.MethodWithParamsFromBody)).First().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Body);
    }

    [Fact]
    public void ShouldParseParamBindingSourceFormHeaderAttribute()
    {
        var parsedParam = new EndpointParser().ParseControllerOperationEndpoints<ParamsController>(nameof(ParamsController.MethodWithParamsFromHeaders)).First().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Header);
    }

    [Fact]
    public void ShouldParseParamsBindingSourceFromFormAttribute()
    {
        var parsedParam = new EndpointParser().ParseControllerOperationEndpoints<ParamsController>(nameof(ParamsController.MethodWithParamsFromForm)).First().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Form);
    }
}

internal class ParamsController : Controller
{
    public void MethodWithParams(string param1, string param2) { }
    public void MethodWithParamsFromQuery([FromQuery] string param1) { }
    public void MethodWithParamsFromBody([FromBody] int param1) { }
    public void MethodWithParamsFromRoute([FromRoute] string param1) { }
    public void MethodWithParamsFromHeaders([FromHeader] string param1) { }
    public void MethodWithParamsFromForm([FromForm] string param1) { }
}

