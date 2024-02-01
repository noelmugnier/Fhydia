using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fhydia.ControllerActions;

public class HyperMediaResultFilter : IAsyncAlwaysRunResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is not ObjectResult objectResult)
            return next();

        var resultValue = objectResult.Value;
        if (resultValue is null)
            return next();

        if(!context.HttpContext.RequestAcceptsHyperMediaTypes())
            return next();

        objectResult.Value = objectResult.Value.ToExpando();
        return next();
    }
}