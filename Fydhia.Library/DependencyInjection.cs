using System.Text.Json;
using Fydhia.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static FhydiaBuilder AddFhydia(this IServiceCollection services)
    {
        var builder = new FhydiaBuilder(services);
        return builder;
    }

    public static IApplicationBuilder UseFhydia(this IApplicationBuilder app)
    {
        return app;
    }
}

public class FhydiaBuilder
{
    private readonly IServiceCollection _serviceCollection;

    private readonly JsonHyperMediaOutputFormatter _jsonHyperMediaOutputFormatter =
        new JsonHyperMediaOutputFormatter(new JsonSerializerOptions());

    public FhydiaBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        _serviceCollection.AddControllers(c => c.Filters.Add<HyperMediaResultFilter>());
        _serviceCollection.AddSingleton(_jsonHyperMediaOutputFormatter);
        _serviceCollection.Configure<MvcOptions>(options =>
        {
            options.OutputFormatters.Insert(0, _jsonHyperMediaOutputFormatter);
        });
    }

    public FhydiaBuilder AddHalJson()
    {
        _jsonHyperMediaOutputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/hal+json"));
        return this;
    }

    public FhydiaBuilder AddJsonLdOutputFormatter()
    {
        _jsonHyperMediaOutputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ld+json"));
        return this;
    }

    public FhydiaBuilder AddCollectionJsonOutputFormatter()
    {
        _jsonHyperMediaOutputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/collection+json"));
        return this;
    }

    public IServiceCollection Build()
    {
        return _serviceCollection;
    }
}