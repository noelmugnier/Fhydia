using Fydhia.Library;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class EndpointParserNameTests
{
    [Fact]
    public void ShouldReturnMethodNameWhenNoAttributeUsed()
    {
        var parsedOperation = new ControllerParser<NamingController>().ParseEndpoints(nameof(NamingController.MethodWithoutName)).First();
        Check.That(parsedOperation.Name).IsEqualTo(typeof(NamingController).FullName + "." + nameof(NamingController.MethodWithoutName));
    }

    [Fact]
    public void ShouldReturnNameFromActionNameAttributeValue()
    {
        var parsedOperation = new ControllerParser<NamingController>().ParseEndpoints(nameof(NamingController.MethodWithActionNameAttribute)).First();
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