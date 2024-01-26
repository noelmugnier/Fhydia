using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class LinkFormatter
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    public LinkFormatter(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }
    
    public string FormatActionLink(string controllerName, string actionName, object? routeValues = null, LinkOptions? options = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            throw new InvalidOperationException();

        var link = _linkGenerator.GetPathByAction(httpContext, actionName, controllerName, new RouteValueDictionary(routeValues), options: options ?? new LinkOptions { LowercaseUrls = true, LowercaseQueryStrings = true });
        if (link is null)
            throw new InvalidOperationException();

        return link;
    }
}