using Microsoft.AspNetCore.Http;

namespace Fydhia.Core.Extensions;

public static class ResultExtensions
{
    public static IResult HyperMedia<T>(this IResultExtensions _, T result)
        => new HyperMediaResult(result);
}