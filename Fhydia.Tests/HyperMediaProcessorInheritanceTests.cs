using System.Reflection;
using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorInheritanceTests
{
    [Fact]
    public void ShouldParseMethodInChildAndSimpleParent()
    {
        var processor = new Processor();
        var parsedOperations = processor.ParseController(typeof(ChildWithSimpleParentController).GetTypeInfo());

        Check.That(parsedOperations).HasSize(2);
    }

    [Fact]
    public void ShouldParseMethodInChildAndAbstractParent()
    {
        var processor = new Processor();
        var parsedOperations = processor.ParseController(typeof(ChildWithAbstractParentController).GetTypeInfo());

        Check.That(parsedOperations).HasSize(2);
    }

    [Fact]
    public void ShouldNotParseMethodDirectlyFromAbstractController()
    {
        var processor = new Processor();
        var parsedOperations = processor.ParseController(typeof(AbstractParentController).GetTypeInfo());

        Check.That(parsedOperations).HasSize(0);
    }

    [Fact]
    public void ShouldParseMethodInChildWithChildControllerName()
    {
       var processor = new Processor();
       var parsedOperations = processor.ParseController(typeof(ChildWithSimpleParentController).GetTypeInfo()).FirstOrDefault(c => c.MethodInfo.Name == nameof(ChildWithSimpleParentController.MethodInChild));

       Check.That(parsedOperations.Template.ToString()).IsEqualTo("ChildWithSimpleParent/MethodInChild");
    }

    [Fact]
    public void ShouldParseMethodInParentWithChildControllerName()
    {
       var processor = new Processor();
       var parsedOperations = processor.ParseController(typeof(ChildWithSimpleParentController).GetTypeInfo()).FirstOrDefault(c => c.MethodInfo.Name == nameof(ChildWithSimpleParentController.MethodInSimpleParent));

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
