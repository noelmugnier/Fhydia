using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Library;

public class HyperMediaResultFilter : IAsyncAlwaysRunResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is not ObjectResult objectResult)
            return next();

        var resultValue = objectResult.Value;
        if (resultValue is null)
            return next();

        var acceptHeader = context.HttpContext.Request.Headers[HeaderNames.Accept];
        if(!acceptHeader.Contains("application/hal+json"))
            return next();

        var expando = objectResult.Value.ToExpando();
        expando.TryAdd("_type", objectResult.Value!.GetType());

        objectResult.Value = expando;
        return next();
    }
}