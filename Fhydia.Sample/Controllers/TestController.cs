using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Fhydia.Sample.Controllers;

[Route("api/controller-actions")]
public class TestController : Controller
{
    [HttpGet("{id}", Name = "ActionRoute")]
    public ActionResult GetFromRouteParam([FromRoute] int id)
    {
        return Ok(
        new CustomReturnType
            { Id = Guid.NewGuid(), Value = "GetFromRouteParam", Count = 1, Inner = new() { Uber = new() } });
    }

    [HttpGet("query", Name = "ActionQuery")]
    public Results<Ok<List<CustomReturnType>>, BadRequest> GetFromQueryParam([FromQuery(Name = "temp")] int id)
    {
        return TypedResults.Ok(new List<CustomReturnType>
            { new() { Id = Guid.NewGuid(), Value = "GetFromQueryParam", Count = 2, Inner = new() } });
    }

    [HttpGet("headers")]
    [EndpointName("SuperTest")]
    public IEnumerable<CustomReturnType> GetFromHeaderParams([FromHeader(Name = "HeaderName")] int id)
    {
        return new List<CustomReturnType>
            { new() { Id = Guid.NewGuid(), Value = "GetFromHeaderParams", Count = 3, Inner = new() } };
    }
}

public class CustomReturnType
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public int Count { get; set; }
    public string CountAsString => Count.ToString();
    public SubType Inner { get; set; }
    public Other OtherObj { get; set; } = new Other() { Id = "test other" };
    public CustomReturnType SubInner => Inner?.Uber;
    public void Meth(){}
}

public class SubType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Value { get; set; } = "Default Value";
    public int Count { get; set; } = 5;
    public CustomReturnType Uber { get; set; }
}

public class Other
{
    public string Id { get; set; }
}