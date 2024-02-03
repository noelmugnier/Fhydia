using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NFluent;

namespace Fhydia.Tests.EndpointParser;

public class EndpointParserParamsTests
{
    [Fact]
    public void ShouldReturnMethodParamsAsList()
    {
        var parsedParameters = new ControllerEndpointsParser<ParamsController>().Parse(nameof(ParamsController.MethodWithParams)).First<EndpointResource>();
        Check.That(parsedParameters.Parameters).HasSize(2);
    }

    [Fact]
    public void ShouldReturnTypeInfoForParameter()
    {
        var parsedParam = new ControllerEndpointsParser<ParamsController>().Parse(nameof(ParamsController.MethodWithParamsFromQuery)).First<EndpointResource>().Parameters.First();
        Check.That(parsedParam.TypeInfo.AsType()).IsEqualTo(typeof(string));
    }

    [Fact]
    public void ShouldParseParamBindingSourceFormQueryAttribute()
    {
        var parsedParam = new ControllerEndpointsParser<ParamsController>().Parse(nameof(ParamsController.MethodWithParamsFromQuery)).First<EndpointResource>().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Query);
    }

    [Fact]
    public void ShouldParseParamBindingSourceFormRouteAttribute()
    {
        var parsedParam = new ControllerEndpointsParser<ParamsController>().Parse(nameof(ParamsController.MethodWithParamsFromRoute)).First<EndpointResource>().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Path);
    }

    [Fact]
    public void ShouldParseParamBindingSourceFormBodyAttribute()
    {
        var parsedParam = new ControllerEndpointsParser<ParamsController>().Parse(nameof(ParamsController.MethodWithParamsFromBody)).First<EndpointResource>().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Body);
    }

    [Fact]
    public void ShouldParseParamBindingSourceFormHeaderAttribute()
    {
        var parsedParam = new ControllerEndpointsParser<ParamsController>().Parse(nameof(ParamsController.MethodWithParamsFromHeaders)).First<EndpointResource>().Parameters.First();
        Check.That(parsedParam.BindingSource).IsEqualTo(BindingSource.Header);
    }

    [Fact]
    public void ShouldParseParamsBindingSourceFromFormAttribute()
    {
        var parsedParam = new ControllerEndpointsParser<ParamsController>().Parse(nameof(ParamsController.MethodWithParamsFromForm)).First<EndpointResource>().Parameters.First();
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