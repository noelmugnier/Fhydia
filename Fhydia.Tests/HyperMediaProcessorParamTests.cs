using System.Reflection;
using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Extensions;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorParamTests
{
    [Fact]
    public void ShouldParseParams()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ParamsController).GetTypeInfo()).FirstOrDefault(c => c.MethodInfo.Name == nameof(ParamsController.MethodWithParams));
        Check.That(parsedOperation.Parameters).HasSize(2);
    }

    [Fact]
    public void ShouldParseParamsBindingSourceAsQuery()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ParamsController).GetTypeInfo()).FirstOrDefault(c => c.MethodInfo.Name == nameof(ParamsController.MethodWithParamsFromQuery));

        var param = parsedOperation.Parameters.FirstOrDefault();
        Check.That(param.BindingSource).IsEqualTo(BindingSource.Query);
    }

    [Fact]
    public void ShouldParseParamsBindingSourceAsPath()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ParamsController).GetTypeInfo()).FirstOrDefault(c => c.MethodInfo.Name == nameof(ParamsController.MethodWithParamsFromRoute));

        var param = parsedOperation.Parameters.FirstOrDefault();
        Check.That(param.BindingSource).IsEqualTo(BindingSource.Path);
    }

    [Fact]
    public void ShouldParseParamsBindingSourceAsBody()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ParamsController).GetTypeInfo()).FirstOrDefault(c => c.MethodInfo.Name == nameof(ParamsController.MethodWithParamsFromBody));

        var param = parsedOperation.Parameters.FirstOrDefault();
        Check.That(param.BindingSource).IsEqualTo(BindingSource.Body);
    }

    [Fact]
    public void ShouldParseParamsBindingSourceAsHeader()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ParamsController).GetTypeInfo()).FirstOrDefault(c => c.MethodInfo.Name == nameof(ParamsController.MethodWithParamsFromHeaders));

        var param = parsedOperation.Parameters.FirstOrDefault();
        Check.That(param.BindingSource).IsEqualTo(BindingSource.Header);
    }

    [Fact]
    public void ShouldParseParamsBindingSourceAsForm()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ParamsController).GetTypeInfo()).FirstOrDefault(c => c.MethodInfo.Name == nameof(ParamsController.MethodWithParamsFromForm));

        var param = parsedOperation.Parameters.FirstOrDefault();
        Check.That(param.BindingSource).IsEqualTo(BindingSource.Form);
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

