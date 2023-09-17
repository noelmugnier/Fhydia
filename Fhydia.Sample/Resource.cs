using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fhydia.Sample;

[Route("api/resource")]
public class Resource : Controller
{
    [HttpGet("test")]
    public Test Get()
    {
        return new Test { Name = "test", Message = "Hello World" };
    }
}

public class Test
{
    public string Message { get; set; }
    public string Name { get; set; }
}

public class FhydiaResultFilter : IAsyncAlwaysRunResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var processor = context.HttpContext.RequestServices.GetRequiredService<FhydiaEnricher>();
        context.Result = processor.EnrichResult(context.Result);
        return next();
    }
}

public static class FhydiaExtensions
{
    public static IServiceCollection AddFhydia(this IServiceCollection services)
    {
        services.AddControllers(c => c.Filters.Add<FhydiaResultFilter>());
        services.AddScoped<FhydiaEnricher>();
        services.AddSingleton<FhydiaConfiguration>();
        return services;
    }

    public static IApplicationBuilder UseFhydia(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        return app;
    }
}

public class FhydiaEnricher
{
    private readonly FhydiaConfiguration _config;

    public FhydiaEnricher(FhydiaConfiguration config)
    {
        _config = config;
    }

    public IActionResult EnrichResult(IActionResult result)
    {
        if (result is not ObjectResult objectResult)
            return result;

        var expando = objectResult.Value.ToExpando();

        _config.GetTypeConfigs(objectResult.Value.GetType()).ForEach(config => config.Enrich(expando));

        objectResult.Value = expando;

        return result;
    }
}

public class FhydiaConfiguration
{
    public List<TypeConfig> GetTypeConfigs(Type type)
    {
        return new List<TypeConfig>();
    }
}

public class TypeConfig
{
    public void Enrich(ExpandoObject expando)
    {
    }
}

public static class ObjectExtensions
{
    public static ExpandoObject ToExpando<T>(this T? obj)
    {
        var expando = new ExpandoObject();

        foreach (var propertyInfo in obj.GetType().GetProperties())
        {
            var currentValue = propertyInfo.GetValue(obj);
            expando.TryAdd(propertyInfo.Name, currentValue);
        }

        return expando;
    }

    public static IDictionary<string, object> ToDictionary(this object? obj)
    {
        var dict = new Dictionary<string, object>();

        foreach (var propertyInfo in obj?.GetType().GetProperties())
        {
            var currentValue = propertyInfo.GetValue(obj);
            dict.TryAdd(propertyInfo.Name, currentValue);
        }

        return dict;
    }

    public static IReadOnlyCollection<Link> GetLinks(this IActionResult result)
    {
        return new List<Link>();
    }
}

public record Link(string rel, string href);
