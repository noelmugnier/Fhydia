using Microsoft.AspNetCore.Mvc;

namespace Fhydia.Sample;

[Route("api/[controller]")]
public class TestController : Controller
{
    [HttpGet("{id}", Name = "ActionRoute")]
    public CustomReturnType GetFromRouteParam([FromRoute] int id)
    {
        return new CustomReturnType { Id = Guid.NewGuid(), Value = "GetFromRouteParam", Count = 1, Inner = new(){Uber = new()} };
    }

    [HttpGet("", Name = "ActionQuery")]
    public IEnumerable<CustomReturnType> GetFromQueryParam([FromQuery] int id)
    {
        return new List<CustomReturnType> { new() { Id = Guid.NewGuid(), Value = "GetFromQueryParam", Count = 2, Inner = new() } };
    }

    [HttpGet("")]
    public IEnumerable<CustomReturnType> GetFromHeaderParams([FromHeader] int id)
    {
        return new List<CustomReturnType> { new() { Id = Guid.NewGuid(), Value = "GetFromHeaderParams", Count = 3, Inner = new() } };
    }
}

public class CustomReturnType
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public int Count { get; set; }
    public SubType Inner { get; set; }
}

public class SubType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Value { get; set; } = "Default Value";
    public int Count { get; set; } = 5;
    public CustomReturnType Uber { get; set; }
}