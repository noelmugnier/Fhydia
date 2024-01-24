using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
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

        var hyperMediaConfiguration = context.HttpContext.RequestServices.GetRequiredService<HyperMediaConfiguration>();
        context.HttpContext.Request.Headers.TryGetValue(HeaderNames.Accept, out var acceptHeaders);
        if(!acceptHeaders.Intersect(hyperMediaConfiguration.SupportedMediaTypes).Any())
            return next();

        objectResult.Value = objectResult.Value.ToExpando();
        return next();
    }
}