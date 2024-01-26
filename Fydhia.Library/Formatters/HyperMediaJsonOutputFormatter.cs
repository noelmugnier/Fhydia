using System.Dynamic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Library;

public class HyperMediaJsonOutputFormatter : SystemTextJsonOutputFormatter
{
    public HyperMediaJsonOutputFormatter(JsonSerializerOptions serializerOptions) : base(serializerOptions)
    {
        SupportedMediaTypes.Clear();
    }

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        if(context.HttpContext.Request.AcceptMediaTypes(SupportedMediaTypes))
            return true;

        return base.CanWriteResult(context);
    }

    protected override bool CanWriteType(Type? type)
    {
        if(type != typeof(ExpandoObject))
            return false;

        return base.CanWriteType(type);
    }

    public override Task WriteAsync(OutputFormatterWriteContext context)
    {
        var hyperMediaEnricher = context.HttpContext.RequestServices.GetRequiredService<HyperMediaJsonEnricher>();
        hyperMediaEnricher.Enrich((ExpandoObject)context.Object!, context.HttpContext.Request.GetAcceptedMediaTypes());

        return base.WriteAsync(context);
    }
}