using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Library;

public abstract class HypermediaTypeFormatter
{
    public abstract ExpandoObject Format(ExpandoObject value, TypeEnricherConfiguration typeEnricherConfiguration);
}

public static class HypermediaTypeFormatterFactory
{
    public static HypermediaTypeFormatter Create(HttpContext httpContext, LinkGenerator linkGenerator)
    {
        var acceptHeader = httpContext.Request.Headers[HeaderNames.Accept].FirstOrDefault();
        if (acceptHeader is null)
            throw new NotSupportedException();

        var acceptedMediaTypes = acceptHeader.Split(',');
        HypermediaTypeFormatter? formatter = null;

        foreach (var acceptedMediaType in acceptedMediaTypes)
        {
            if (acceptedMediaType != "application/hal+json")
                continue;

            formatter = new JsonHalTypeFormatter(httpContext, linkGenerator);
            break;
        }

        if (formatter is null)
            throw new NotSupportedException();

        return formatter;
    }
}