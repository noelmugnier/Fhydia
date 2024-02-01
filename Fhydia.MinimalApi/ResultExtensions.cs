using Microsoft.AspNetCore.Http;

namespace Fhydia.MinimalApi;

public static class ResultExtensions
{
    public static IResult HyperMedia<T>(this IResultExtensions _, T result)
        => new HyperMediaResult<T>(result);
}