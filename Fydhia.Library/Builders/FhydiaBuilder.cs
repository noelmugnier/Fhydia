using System.Text.Json;
using Fydhia.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Microsoft.Extensions.DependencyInjection;

public class FhydiaBuilder
{
    private readonly IServiceCollection _serviceCollection;
    private readonly HyperMediaJsonOutputFormatter _hyperMediaJsonOutputFormatter;

    internal FhydiaBuilder(IServiceCollection serviceCollection, JsonSerializerOptions serializerOptions)
    {
        //be sure to be consistent between IDictionary (expando or dynamic) and object property naming
        if(serializerOptions.DictionaryKeyPolicy != serializerOptions.PropertyNamingPolicy)
            serializerOptions.DictionaryKeyPolicy = serializerOptions.PropertyNamingPolicy;

        _hyperMediaJsonOutputFormatter = new HyperMediaJsonOutputFormatter(serializerOptions);

        _serviceCollection = serviceCollection;
        _serviceCollection.AddHttpContextAccessor();
        _serviceCollection.AddScoped<IHyperMediaJsonEnricher, HyperMediaJsonEnricher>();
        _serviceCollection.AddScoped<IProvideHyperMediaTypeFormatter, HyperMediaTypeFormatterProvider>();
        _serviceCollection.Configure<MvcOptions>(options =>
        {
            options.OutputFormatters.Insert(0, _hyperMediaJsonOutputFormatter);
        });
        _serviceCollection.AddControllers(c => c.Filters.Add<HyperMediaResultFilter>());
    }

    public FhydiaBuilder AddHalJsonSupport()
    {
        _hyperMediaJsonOutputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/hal+json"));
        return this;
    }

    public FhydiaBuilder AddJsonLd()
    {
        _hyperMediaJsonOutputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ld+json"));
        return this;
    }

    public FhydiaBuilder AddCollectionJson()
    {
        _hyperMediaJsonOutputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/collection+json"));
        return this;
    }

    public FhydiaBuilder Configure(Action<HyperMediaConfigurationBuilder> configure)
    {
        var builder = new HyperMediaConfigurationBuilder(_hyperMediaJsonOutputFormatter.SerializerOptions, _hyperMediaJsonOutputFormatter.SupportedMediaTypes);
        configure.Invoke(builder);

        _serviceCollection.AddSingleton(builder.Build());
        return this;
    }
}