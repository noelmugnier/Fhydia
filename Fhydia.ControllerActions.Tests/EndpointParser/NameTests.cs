using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests.EndpointParser;

public class EndpointParserNameTests
{
    [Fact]
    public void ShouldReturnMethodNameWhenNoAttributeUsed()
    {
        var parsedOperation = new ControllerEndpointsParser<NamingController>().Parse(nameof(NamingController.MethodWithoutName)).First<EndpointResource>();
        Check.That(parsedOperation.Name).IsEqualTo(typeof(NamingController).FullName + "." + nameof(NamingController.MethodWithoutName));
    }

    [Fact]
    public void ShouldReturnNameFromActionNameAttributeValue()
    {
        var parsedOperation = new ControllerEndpointsParser<NamingController>().Parse(nameof(NamingController.MethodWithActionNameAttribute)).First<EndpointResource>();
        Check.That(parsedOperation.Name).IsEqualTo(typeof(NamingController).FullName + "." +"action-name");
    }
}

internal class NamingController : Controller
{
    public void MethodWithoutName() { }

    [HttpGet(Name = "http-name")]
    public void MethodWithHttpAttributeName() { }

    [Route("", Name = "route-name")]
    public void MethodWithRouteAttributeName() { }

    [ActionName("action-name")]
    public void MethodWithActionNameAttribute() { }
}