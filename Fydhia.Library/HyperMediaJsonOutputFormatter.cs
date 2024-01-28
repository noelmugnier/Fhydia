using System.Dynamic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace Fydhia.Library;

public class HyperMediaJsonOutputFormatter : SystemTextJsonOutputFormatter
{
    public HyperMediaJsonOutputFormatter(JsonSerializerOptions serializerOptions, MediaTypeCollection mediaTypeCollection) : base(serializerOptions)
    {
        SupportedMediaTypes.Clear();

        foreach (var mediaType in mediaTypeCollection)
        {
            SupportedMediaTypes.Add(mediaType);
        }
    }

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        return context.HttpContext.Request.AcceptMediaTypes(SupportedMediaTypes) || base.CanWriteResult(context);
    }

    protected override bool CanWriteType(Type? type)
    {
        return type == typeof(ExpandoObject) && base.CanWriteType(type);
    }

    public override Task WriteAsync(OutputFormatterWriteContext context)
    {
        var hyperMediaEnricher = context.HttpContext.RequestServices.GetRequiredService<IHyperMediaJsonEnricher>();
        hyperMediaEnricher.Enrich((ExpandoObject)context.Object!);

        var hyperMediaTypeFormatterProvider = context.HttpContext.RequestServices.GetRequiredService<IProvideHyperMediaTypeFormatter>();
        var formatter = hyperMediaTypeFormatterProvider.GetFormatter(context.HttpContext.Request.GetAcceptedMediaTypes());
        formatter.Format((ExpandoObject)context.Object!, context.HttpContext);

        return base.WriteAsync(context);
    }
}