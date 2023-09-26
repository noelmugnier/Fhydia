using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorGroupTests
{
    [Fact]
    public void ShouldUseControllerNameIfNoApiExplorerAttributeFoundOnMethodAndController()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithoutApiExplorerAttributeController>(nameof(WithoutApiExplorerAttributeController.MethodWithoutAttributes)).First();
        Check.That(parsedOperation.Group.Name).IsEqualTo("WithoutApiExplorerAttribute");
    }

    [Fact]
    public void ShouldUseApiExplorerGroupNameValueFromControllerIfNotOnMethod()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithApiExplorerAttributeController>(nameof(WithApiExplorerAttributeController.MethodWithoutAttributes)).First();
        Check.That(parsedOperation.Group.Name).IsEqualTo("ControllerAttr");
    }

    [Fact]
    public void ShouldUseApiExplorerGroupNameValueFromMethod()
    {
        var parsedOperation = new EndpointParser().ParseControllerOperationEndpoints<WithApiExplorerAttributeController>(nameof(WithApiExplorerAttributeController.MethodWithAttribute)).First();
        Check.That(parsedOperation.Group.Name).IsEqualTo("MethodAttr");
    }
}

[ApiExplorerSettings(GroupName = "ControllerAttr")]
internal class WithApiExplorerAttributeController : Controller
{
    public void MethodWithoutAttributes() { }

    [ApiExplorerSettings(GroupName = "MethodAttr")]
    public void MethodWithAttribute() { }
}

internal class WithoutApiExplorerAttributeController : Controller
{
    public void MethodWithoutAttributes() { }
}
