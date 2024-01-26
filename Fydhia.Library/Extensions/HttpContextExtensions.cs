using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Library;

public static class HttpContextExtensions
{
    public static IEnumerable<string> GetAcceptedMediaTypes(this HttpRequest request)
    {
        return request.Headers[HeaderNames.Accept].SelectMany(header => header.Split(';'));
    }
    
    public static bool AcceptMediaTypes(this HttpRequest request, IEnumerable<string> mediaTypes)
    {
        var acceptedMediaTypes = request.GetAcceptedMediaTypes();
        return acceptedMediaTypes.Intersect(mediaTypes).Any();
    }
}