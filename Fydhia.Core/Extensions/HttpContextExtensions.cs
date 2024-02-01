using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Core.Extensions;

public static class HttpContextExtensions
{
    public static IEnumerable<string> GetAcceptedMediaTypes(this HttpRequest request)
    {
        return request.Headers[HeaderNames.Accept].SelectMany(header => header.Split(';'));
    }
    
    public static bool RequestAcceptsHyperMediaTypes(this HttpContext httpContext)
    {
        var hyperMediaConfiguration = httpContext.RequestServices.GetRequiredService<HyperMediaConfiguration>();
        return httpContext.Request.AcceptMediaTypes(hyperMediaConfiguration.SupportedMediaTypes);
    }

    public static bool AcceptMediaTypes(this HttpRequest httpRequest, MediaTypeCollection supportedMediaTypes)
    {
        var acceptedMediaTypes = httpRequest.GetAcceptedMediaTypes();
        return acceptedMediaTypes.Intersect(supportedMediaTypes).Any();
    }
}