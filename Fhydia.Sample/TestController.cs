using Microsoft.AspNetCore.Mvc;

namespace Fhydia.Sample;

[Route("api/[controller]")]
public class TestController : Controller
{
    [HttpGet("{id}", Name = "ActionRoute")]
    public CustomReturnType GetFromRouteParam([FromRoute] int id)
    {
        return new CustomReturnType { Id = id, Name = "test", Message = "Hello World" };
    }

    [HttpGet("", Name = "ActionQuery")]
    public IEnumerable<CustomReturnType> GetFromQueryParam([FromQuery] int id)
    {
        return new List<CustomReturnType> { new() { Id = id, Name = "test", Message = "Hello World" } };
    }

    [HttpGet("")]
    public IEnumerable<CustomReturnType> GetFromHeaderParams([FromHeader] int id)
    {
        return new List<CustomReturnType> { new() { Id = id, Name = "test", Message = "Hello World" } };
    }
}

public class CustomReturnType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
}