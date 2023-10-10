using Fydhia.Library;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class EndpointTests
{
    [Fact]
    public void ShouldReturnOneGetEndpointWhenUsingOneRouteAttribute()
    {
        var endpointParser = new ControllerParser<EndpointController>();
        var endpoints = endpointParser.ParseEndpoints();

        Check.That(endpoints.Count(c => c.MethodName == nameof(EndpointController.MethodWithSingleRouteAttribute))).IsEqualTo(1);
        Check.That(endpoints.First(c => c.MethodName == nameof(EndpointController.MethodWithSingleRouteAttribute)).Verb).IsEqualTo(HttpVerb.GET);
    }

    [Fact]
    public void ShouldReturnOneGetEndpointWhenUsingOneHttpGetAttribute()
    {
        var endpointParser = new ControllerParser<EndpointController>();
        var endpoints = endpointParser.ParseEndpoints();

        Check.That(endpoints.Count(c => c.MethodName == nameof(EndpointController.MethodWithSingleHttpAttribute))).IsEqualTo(1);
        Check.That(endpoints.First(c => c.MethodName == nameof(EndpointController.MethodWithSingleHttpAttribute)).Verb).IsEqualTo(HttpVerb.GET);
    }

    [Fact]
    public void ShouldReturnTwoGetEndpointsWhenUsingTwoHttpAttributes()
    {
       var endpointParser = new ControllerParser<EndpointController>();
       var endpoints = endpointParser.ParseEndpoints(nameof(EndpointController.MethodWithTwoRouteAttribute));

       Check.That(endpoints.Count(c => c.MethodName == nameof(EndpointController.MethodWithTwoRouteAttribute))).IsEqualTo(2);
    }
}

[Route("api")]
internal class EndpointController : Controller
{
    [Route("singleroute")]
    public void MethodWithSingleRouteAttribute() { }

    [HttpGet("httpsingleroute")]
    public void MethodWithSingleHttpAttribute() { }

    [HttpGet("tworoutes")]
    [HttpPost("tworoutes")]
    public void MethodWithTwoRouteAttribute() { }
}