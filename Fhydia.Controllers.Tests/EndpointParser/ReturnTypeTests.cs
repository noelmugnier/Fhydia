using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests.EndpointParser;

public class EndpointParserReturnTypeTests
{
    [Fact]
    public void ShouldReturnResultTypeInfoForResult()
    {
        var parsedOperation = new ControllerEndpointsParser<ReturnController>().Parse(nameof(ReturnController.MethodWithReturnType)).First<EndpointResource>();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldReturnResultTypeFromGenericActionResultType()
    {
        var parsedOperation = new ControllerEndpointsParser<ReturnController>().Parse(nameof(ReturnController.MethodWithActionResultTyped)).First<EndpointResource>();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldReturnGenericResultType()
    {
        var parsedOperation = new ControllerEndpointsParser<ReturnController>().Parse(nameof(ReturnController.MethodWithGenericTypeReturnType)).First<EndpointResource>();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(GenericType<MyReturnType>));
    }

    [Fact]
    public void ShouldReturnResultTypeFromMethodReturnTypeWhenTypeIsKnownEvenIfUsingProducesResponseTypeAttribute()
    {
        var parsedOperation = new ControllerEndpointsParser<ReturnController>().Parse(nameof(ReturnController.MethodWithTypedActionResultAndProducesResponseTypeAttribute)).First<EndpointResource>();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldReturnResultTypeFromGenericTaskType()
    {
        var parsedOperation = new ControllerEndpointsParser<ReturnController>().Parse(nameof(ReturnController.MethodWithTaskReturnType)).First<EndpointResource>();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldReturnResultTypeFromGenericValueTaskType()
    {
        var parsedOperation = new ControllerEndpointsParser<ReturnController>().Parse(nameof(ReturnController.MethodWithValueTaskReturnType)).First<EndpointResource>();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldReturnGenericResultTypeFromGenericActionResultType()
    {
        var parsedOperation = new ControllerEndpointsParser<ReturnController>().Parse(nameof(ReturnController.MethodWithTaskActionResultGenericTyped)).First<EndpointResource>();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(GenericType<MyReturnType>));
    }
}

internal class ReturnErrorController : Controller
{
    public ActionResult MethodWithActionResultWithoutType()
    {
        return Ok();
    }
}

internal class ReturnController : Controller
{
    public MyReturnType MethodWithReturnType()
    {
        return new MyReturnType();
    }

    public GenericType<MyReturnType> MethodWithGenericTypeReturnType()
    {
        return new GenericType<MyReturnType>();
    }

    [ProducesResponseType(typeof(MyReturnType), 200)]
    public ActionResult MethodWithActionResultWithProducesResponseTypeAttribute()
    {
        return Ok();
    }

    public ActionResult<MyReturnType> MethodWithActionResultTyped()
    {
        return new MyReturnType();
    }

    public Task<MyReturnType> MethodWithTaskReturnType()
    {
        return Task.FromResult(new MyReturnType());
    }

    public ValueTask<MyReturnType> MethodWithValueTaskReturnType()
    {
        return ValueTask.FromResult(new MyReturnType());
    }

    public async Task<ActionResult<GenericType<MyReturnType>>> MethodWithTaskActionResultGenericTyped()
    {
        return Ok(new GenericType<MyReturnType>());
    }

    [ProducesResponseType(typeof(MyReturnType), 200)]
    public async Task<ActionResult> MethodWithTaskActionResultWithProducesResponseTypeAttribute()
    {
        return Ok();
    }

    [ProducesResponseType(typeof(string), 200)]
    public ActionResult<MyReturnType> MethodWithTypedActionResultAndProducesResponseTypeAttribute()
    {
        return Ok(new MyReturnType());
    }
}

public class GenericType<T>
{
}

internal class MyReturnType
{
    public string MyProperty { get; set; }
}