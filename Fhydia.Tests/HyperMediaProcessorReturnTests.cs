using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorReturnTests
{
    [Fact]
    public void ShouldAssignOperationControllerTypeAsRelatedType()
    {
        var parsedOperation = new EndpointParser().ParseControllerEndpoints<ReturnController>().First();
        Check.That(parsedOperation.TypeInfo.AsType()).IsEqualTo(typeof(ReturnController));
    }

    [Fact]
    public void ShouldAssignOperationReturnTypeWhenUsingSimpleType()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<ReturnController>(nameof(ReturnController.MethodWithReturnType)).First();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignOperationReturnInnerTypeWhenUsingActionResultTyped()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<ReturnController>(nameof(ReturnController.MethodWithActionResultTyped)).First();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignOperationReturnGenericTypeWhenNotUsingActionResult()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<ReturnController>(nameof(ReturnController.MethodWithGenericTypeReturnType)).First();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(GenericType<MyReturnType>));
    }

    [Fact]
    public void ShouldThrowExceptionWhenUsingActionResultUntypedWithoutProducesResponseTypeAttribute()
    {
        Assert.Throws<InvalidOperationException>(() => new EndpointParser().ParseControllerEndpoints<ReturnErrorController>());
    }

    [Fact]
    public void ShouldAssignReturnedTypeFromProducesResponseTypeAttribute()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<ReturnController>(nameof(ReturnController.MethodWithActionResultWithProducesResponseTypeAttribute)).First();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignReturnedTypeFromTaskInnerType()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<ReturnController>(nameof(ReturnController.MethodWithTaskReturnType)).First();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignReturnedTypeFromValueTaskInnerType()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<ReturnController>(nameof(ReturnController.MethodWithValueTaskReturnType)).First();
        Check.That(parsedOperation.Result.TypeInfo.AsType()).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignReturnedTypeFromTaskActionResultInnerType()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<ReturnController>(nameof(ReturnController.MethodWithTaskActionResultGenericTyped)).First();
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
}

public class GenericType<T>
{
}

internal class MyReturnType
{
    public string MyProperty { get; set; }
}
