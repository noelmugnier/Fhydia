using Fhydia.Controllers.Resources;
using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests.EndpointParser;

public class EndpointParserGroupTests
{
    [Fact]
    public void ShouldUseControllerNameWhenApiExplorerAttributeNotFound()
    {
        var parsedOperation = new ControllerEndpointsParser<WithoutApiExplorerAttributeController>().Parse(nameof(WithoutApiExplorerAttributeController.MethodWithoutAttributes)).First<EndpointResource>();
        Check.That<string>(((ControllerEndpoint)parsedOperation).Group.Name).IsEqualTo("WithoutApiExplorerAttribute");
    }

    [Fact]
    public void ShouldUseApiExplorerGroupNameValueFromControllerWhenNotFoundOnMethod()
    {
        var parsedOperation = new ControllerEndpointsParser<WithApiExplorerAttributeController>().Parse(nameof(WithApiExplorerAttributeController.MethodWithoutAttributes)).First<EndpointResource>();
        Check.That<string>(((ControllerEndpoint)parsedOperation).Group.Name).IsEqualTo("ControllerAttr");
    }

    [Fact]
    public void ShouldUseApiExplorerGroupNameValueFromMethodWhenPresentOnMethodAndController()
    {
        var parsedOperation = new ControllerEndpointsParser<WithApiExplorerAttributeController>().Parse(nameof(WithApiExplorerAttributeController.MethodWithAttribute)).First<EndpointResource>();
        Check.That<string>(((ControllerEndpoint)parsedOperation).Group.Name).IsEqualTo("MethodAttr");
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