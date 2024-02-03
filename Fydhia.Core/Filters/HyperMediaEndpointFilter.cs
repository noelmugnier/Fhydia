using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fydhia.Core.Filters;

public class HyperMediaEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if(!context.HttpContext.RequestAcceptsHyperMediaTypes())
            return await next(context);

        var result = await next(context);
        if (result is not ObjectResult objectResult)
            return result;
        
        return new HyperMediaResult(objectResult.Value);
    }
}