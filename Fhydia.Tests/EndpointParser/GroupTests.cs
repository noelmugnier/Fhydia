using Fydhia.Library;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class EndpointParserGroupTests
{
    [Fact]
    public void ShouldUseControllerNameWhenApiExplorerAttributeNotFound()
    {
        var parsedOperation = new ControllerParser<WithoutApiExplorerAttributeController>().ParseEndpoints(nameof(WithoutApiExplorerAttributeController.MethodWithoutAttributes)).First();
        Check.That(parsedOperation.Group.Name).IsEqualTo("WithoutApiExplorerAttribute");
    }

    [Fact]
    public void ShouldUseApiExplorerGroupNameValueFromControllerWhenNotFoundOnMethod()
    {
        var parsedOperation = new ControllerParser<WithApiExplorerAttributeController>().ParseEndpoints(nameof(WithApiExplorerAttributeController.MethodWithoutAttributes)).First();
        Check.That(parsedOperation.Group.Name).IsEqualTo("ControllerAttr");
    }

    [Fact]
    public void ShouldUseApiExplorerGroupNameValueFromMethodWhenPresentOnMethodAndController()
    {
        var parsedOperation = new ControllerParser<WithApiExplorerAttributeController>().ParseEndpoints(nameof(WithApiExplorerAttributeController.MethodWithAttribute)).First();
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