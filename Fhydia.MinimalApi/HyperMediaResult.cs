using System.Text.Json;
using Fydhia.Core;
using Fydhia.Core.Configurations;
using Fydhia.Core.Extensions;
using Fydhia.Core.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Fhydia.MinimalApi;

public class HyperMediaResult<T> : IResult
{
    private readonly T _result;

    public HyperMediaResult(T result)
    {
        _result = result;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (!httpContext.RequestAcceptsHyperMediaTypes())
        {
            await SerializeResult(httpContext, _result!, "application/json");
            return;
        }
        
        var expando = _result.ToExpando();

        var hyperMediaEnricher = httpContext.RequestServices.GetRequiredService<IHyperMediaJsonEnricher>();
        hyperMediaEnricher.Enrich(expando);

        var hyperMediaTypeFormatterProvider = httpContext.RequestServices.GetRequiredService<IProvideHyperMediaTypeFormatter>();
        var formatter = hyperMediaTypeFormatterProvider.GetFormatter(httpContext.Request.GetAcceptedMediaTypes());
        formatter.Format(expando, httpContext);
        
        await SerializeResult(httpContext, expando, formatter.FormattedMediaType);
    }

    private static async Task SerializeResult(HttpContext httpContext, object result,
        string contentType)
    {
        await using var ms = new FileBufferingWriteStream();
        var serializerOptions = httpContext.RequestServices.GetRequiredService<HyperMediaConfiguration>().JsonSerializerOptions;
        
        await JsonSerializer.SerializeAsync(ms, result, serializerOptions, httpContext.RequestAborted);

        httpContext.Response.ContentType = contentType;
        await ms.DrainBufferAsync(httpContext.Response.Body, httpContext.RequestAborted);
    }
}