using System.Reflection;
using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorReturnTests
{
    [Fact]
    public void ShouldAssignOperationControllerTypeAsRelatedType()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ReturnController).GetTypeInfo()).FirstOrDefault();

        Check.That(parsedOperation.RelatedType).IsEqualTo(typeof(ReturnController));
    }

    [Fact]
    public void ShouldAssignOperationReturnTypeWhenUsingSimpleType()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ReturnController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(ReturnController.MethodWithReturnType));

        Check.That(parsedOperation.Result.RelatedType).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignOperationReturnInnerTypeWhenUsingActionResultTyped()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ReturnController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(ReturnController.MethodWithActionResultTyped));

        Check.That(parsedOperation.Result.RelatedType).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignOperationReturnGenericTypeWhenNotUsingActionResult()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ReturnController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(ReturnController.MethodWithGenericTypeReturnType));

        Check.That(parsedOperation.Result.RelatedType).IsEqualTo(typeof(GenericType<MyReturnType>));
    }

    [Fact]
    public void ShouldThrowExceptionWhenUsingActionResultUntypedWithoutProducesResponseTypeAttribute()
    {
        var processor = new Processor();
        Assert.Throws<InvalidOperationException>(() => processor.ParseController(typeof(ReturnErrorController).GetTypeInfo()));
    }

    [Fact]
    public void ShouldAssignReturnedTypeFromProducesResponseTypeAttribute()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ReturnController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(ReturnController.MethodWithActionResultWithProducesResponseTypeAttribute));

        Check.That(parsedOperation.Result.RelatedType).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignReturnedTypeFromTaskInnerType()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ReturnController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(ReturnController.MethodWithTaskReturnType));
        Check.That(parsedOperation.Result.RelatedType).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignReturnedTypeFromValueTaskInnerType()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ReturnController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(ReturnController.MethodWithValueTaskReturnType));
        Check.That(parsedOperation.Result.RelatedType).IsEqualTo(typeof(MyReturnType));
    }

    [Fact]
    public void ShouldAssignReturnedTypeFromTaskActionResultInnerType()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(ReturnController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(ReturnController.MethodWithTaskActionResultGenericTyped));
        Check.That(parsedOperation.Result.RelatedType).IsEqualTo(typeof(GenericType<MyReturnType>));
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
