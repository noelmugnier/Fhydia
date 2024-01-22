﻿using System.Dynamic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Library;

public class JsonHyperMediaOutputFormatter : SystemTextJsonOutputFormatter
{
    public JsonHyperMediaOutputFormatter(JsonSerializerOptions serializerOptions) : base(serializerOptions)
    {
        SupportedMediaTypes.Clear();
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/hal+json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ld+json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/collection+json"));
    }

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        context.HttpContext.Request.Headers.TryGetValue(HeaderNames.Accept, out var acceptHeader);
        var acceptHeaders = context.HttpContext.Request.Headers[HeaderNames.Accept];
        if(acceptHeaders.Intersect(SupportedMediaTypes).Any())
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
        var hyperMediaEnricher = context.HttpContext.RequestServices.GetRequiredService<HyperMediaEnricher>();
        hyperMediaEnricher.Enrich(context.HttpContext, context.Object as ExpandoObject);

        return base.WriteAsync(context);
    }
}