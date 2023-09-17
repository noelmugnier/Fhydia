using Microsoft.AspNetCore.Mvc.Filters;

namespace Fhydia.Sample;

public class HyperMediaResultFilter : IAsyncAlwaysRunResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var hyperMediaEnricher = context.HttpContext.RequestServices.GetRequiredService<HyperMediaEnricher>();
        context.Result = hyperMediaEnricher.EnrichResult(context.Result);
        return next();
    }
}

