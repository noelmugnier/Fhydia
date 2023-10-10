using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Fydhia.Library;

public class HyperMediaResultFilter : IAsyncAlwaysRunResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var hyperMediaEnricher = context.HttpContext.RequestServices.GetRequiredService<HyperMediaEnricher>();
        if (context.Result is not ObjectResult objectResult)
            return next();

        var resultValue = objectResult.Value;
        if (resultValue is null)
            return next();

        objectResult.Value = hyperMediaEnricher.Enrich(context.HttpContext, resultValue);
        context.Result = objectResult;
        return next();
    }
}