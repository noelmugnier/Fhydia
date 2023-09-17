using Microsoft.AspNetCore.Mvc;

namespace Fhydia.Sample;

public class MyController : Controller
{
    [HttpGet("okte", Name = "GetResource")]
    public ActionResult<MyModel> Get()
    {
        return new MyModel { Name = "test", Message = "Hello World" };
    }

    [HttpGet]
    [Route("myoperation2")]
    public ActionResult Operation1()
    {
        return Ok();
    }
}

public class MyModel
{
    public string Message { get; set; }
    public string Name { get; set; }
}

[Route("test")]
public class RoutingController : Controller
{
    public void MethodWithHttpAttribute() { Console.WriteLine("test"); }

    [HttpGet]
    public void MethodWithHttpAttributeEmpty() { }

    [HttpGet("/HttpRouteOnly")]
    public void MethodWithHttpAttributeNonEmpty() { }

    [Route("re")]
    [HttpPost]
    public void MethodWithRouteAttributeEmpty() { }
}
