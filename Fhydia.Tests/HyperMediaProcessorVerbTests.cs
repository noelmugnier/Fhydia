using System.Reflection;
using Fhydia.Sample;
using Microsoft.AspNetCore.Mvc;
using NFluent;

namespace Fhydia.Tests;

public class HyperMediaProcessorVerbTests
{
    [Fact]
    public void ShouldParseOperationHttpVerbWithGetAsDefaultVerb()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(VerbController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(VerbController.MethodWithoutVerb));

        Check.That(parsedOperation.Method).IsEqualTo(HttpVerb.GET);
    }

    [Fact]
    public void ShouldParseOperationHttpPostVerb()
    {
        var processor = new Processor();
        var parsedOperation = processor.ParseController(typeof(VerbController).GetTypeInfo()).FirstOrDefault(c => c.MethodName == nameof(VerbController.MethodWithHttpGetVerb));

        Check.That(parsedOperation.Method).IsEqualTo(HttpVerb.POST);
    }
}

internal class VerbController : Controller
{
    public void MethodWithoutVerb() { }

    [HttpPost]
    public void MethodWithHttpGetVerb() { }
}

