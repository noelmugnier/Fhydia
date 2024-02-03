using System.Text.Json;
using Fydhia.Core.Enrichers;
using Fydhia.Core.Extensions;
using Fydhia.Core.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fydhia.Core;

public class HyperMediaResult : IResult
{
    private readonly object? _result;

    public HyperMediaResult(object? result)
    {
        _result = result;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (!httpContext.RequestAcceptsHyperMediaTypes() || _result is null)
        {
            await SerializeResult(httpContext, _result!, "application/json");
            return;
        }
        
        var expando = _result.ToExpando();

        var hyperMediaEnricher = httpContext.RequestServices.GetRequiredService<IHyperMediaObjectEnricher>();
        hyperMediaEnricher.Enrich(expando);

        var hyperMediaTypeFormatterProvider = httpContext.RequestServices.GetRequiredService<IProvideHyperMediaTypesFormatter>();
        var formatter = hyperMediaTypeFormatterProvider.GetFormatter(httpContext.Request.GetAcceptedMediaTypes());
        formatter.Format(expando, httpContext);
        
        await SerializeResult(httpContext, expando, formatter.FormattedMediaType);
    }

    private static async Task SerializeResult(HttpContext httpContext, object result, string contentType)
    {
        await using var ms = new FileBufferingWriteStream();
        var serializerOptions = httpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions;
        
        await JsonSerializer.SerializeAsync(ms, result, serializerOptions, httpContext.RequestAborted);

        httpContext.Response.ContentType = contentType;
        await ms.DrainBufferAsync(httpContext.Response.Body, httpContext.RequestAborted);
    }
}