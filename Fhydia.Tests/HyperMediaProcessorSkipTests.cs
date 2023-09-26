using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorSkitTests
{
    [Fact]
    public void ShouldSkipControllerWithNonControllerAttribute()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<NonControllerAttributeController>();
        Check.That(parsedOperations).HasSize(0);
    }

    [Fact]
    public void ShouldSkipMethodsWithNonActionAttribute()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<ControllerWithNonActionAttributeController>();
        Check.That(parsedOperations).HasSize(1);
        Check.That(parsedOperations.First().MethodInfo.Name).IsEqualTo(nameof(ControllerWithNonActionAttributeController.MethodWithoutAttributes));
    }
}

[NonController]
internal class NonControllerAttributeController : Controller
{
    public void Method() { }
}

internal class ControllerWithNonActionAttributeController : Controller
{
    public void MethodWithoutAttributes() { }

    [NonAction]
    public void MethodWithNonActionAttribute() { }
}
