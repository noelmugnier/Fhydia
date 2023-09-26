using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorInheritanceTests
{
    [Fact]
    public void ShouldParseMethodInChildAndSimpleParent()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<ChildWithSimpleParentController>();
        Check.That(parsedOperations).HasSize(2);
    }

    [Fact]
    public void ShouldParseMethodInChildAndAbstractParent()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<ChildWithAbstractParentController>();
        Check.That(parsedOperations).HasSize(2);
    }

    [Fact]
    public void ShouldNotParseMethodDirectlyFromAbstractController()
    {
        var parsedOperations = new EndpointParser().ParseControllerEndpoints<AbstractParentController>();
        Check.That(parsedOperations).HasSize(0);
    }

    [Fact]
    public void ShouldParseMethodInChildWithChildControllerName()
    {
        var parsedOperations = new EndpointParser().ParseControllerOperationEndpoints<ChildWithSimpleParentController>(nameof(ChildWithSimpleParentController.MethodInChild)).First();
        Check.That(parsedOperations.Template.ToString()).IsEqualTo("ChildWithSimpleParent/MethodInChild");
    }

    [Fact]
    public void ShouldParseMethodInParentWithChildControllerName()
    {
        var parsedOperations = new EndpointParser().ParseControllerOperationEndpoints<ChildWithSimpleParentController>(nameof(ChildWithSimpleParentController.MethodInSimpleParent)).First();
        Check.That(parsedOperations.Template.ToString()).IsEqualTo("ChildWithSimpleParent/MethodInSimpleParent");
    }
}

internal abstract class AbstractParentController : Controller
{
    public void MethodInAbstractParent() { }
}

internal class SimpleParentController : Controller
{
    public void MethodInSimpleParent() { }
}

internal class ChildWithSimpleParentController : SimpleParentController
{
    public void MethodInChild() { }
}

internal class ChildWithAbstractParentController : AbstractParentController
{
    public void MethodInChild() { }
}
