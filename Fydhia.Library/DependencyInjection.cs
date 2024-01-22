using System.Text.Json;
using Fydhia.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

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

    public FhydiaBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        _serviceCollection.AddControllers(c => c.Filters.Add<HyperMediaResultFilter>());
    }

    public FhydiaBuilder AddJsonHyperMediaOutputFormatter()
    {
        _serviceCollection.Configure<MvcOptions>(options =>
        {
            options.OutputFormatters.Insert(0, new JsonHyperMediaOutputFormatter(new JsonSerializerOptions()));
        });

        return this;
    }

    public IServiceCollection Build()
    {
        return _serviceCollection;
    }
}