using Fhydia.Controllers.Resources;
using Fydhia.Core.Common;
using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests.EndpointParser;

public class EndpointTests
{
    [Fact]
    public void ShouldReturnOneGetEndpointWhenUsingOneRouteAttribute()
    {
        var endpointParser = new ControllerEndpointsParser<EndpointController>();
        var endpoints = endpointParser.Parse();

        Check.That(endpoints.Count<EndpointResource>(c => ((ControllerEndpoint)c).MethodName == nameof(EndpointController.MethodWithSingleRouteAttribute))).IsEqualTo(1);
        Check.That(endpoints.First<EndpointResource>(c => ((ControllerEndpoint)c).MethodName == nameof(EndpointController.MethodWithSingleRouteAttribute)).Verb).IsEqualTo(HttpVerb.GET);
    }

    [Fact]
    public void ShouldReturnOneGetEndpointWhenUsingOneHttpGetAttribute()
    {
        var endpointParser = new ControllerEndpointsParser<EndpointController>();
        var endpoints = endpointParser.Parse();

        Check.That(endpoints.Count<EndpointResource>(c => ((ControllerEndpoint)c).MethodName == nameof(EndpointController.MethodWithSingleHttpAttribute))).IsEqualTo(1);
        Check.That(endpoints.First<EndpointResource>(c => ((ControllerEndpoint)c).MethodName == nameof(EndpointController.MethodWithSingleHttpAttribute)).Verb).IsEqualTo(HttpVerb.GET);
    }

    [Fact]
    public void ShouldReturnTwoGetEndpointsWhenUsingTwoHttpAttributes()
    {
       var endpointParser = new ControllerEndpointsParser<EndpointController>();
       var endpoints = endpointParser.Parse(nameof(EndpointController.MethodWithTwoRouteAttribute));

       Check.That(endpoints.Count<EndpointResource>(c => ((ControllerEndpoint)c).MethodName == nameof(EndpointController.MethodWithTwoRouteAttribute))).IsEqualTo(2);
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