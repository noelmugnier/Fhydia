using System.Reflection;
using System.Text.Json;
using Fydhia.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Microsoft.Extensions.DependencyInjection;

public class FhydiaBuilder
{
    private readonly IServiceCollection _serviceCollection;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly MediaTypeCollection _mediaTypeCollection = new();
    private readonly HyperMediaConfigurationBuilder _builder;

    internal FhydiaBuilder(IServiceCollection serviceCollection, JsonSerializerOptions serializerOptions)
    {
        _serviceCollection = serviceCollection;

        //be sure to be consistent between IDictionary (expando or dynamic) and object property naming
        if(serializerOptions.DictionaryKeyPolicy != serializerOptions.PropertyNamingPolicy)
            serializerOptions.DictionaryKeyPolicy = serializerOptions.PropertyNamingPolicy;

        _serializerOptions = serializerOptions;
        _builder = new HyperMediaConfigurationBuilder(_serializerOptions, _mediaTypeCollection);
    }

    public FhydiaBuilder AddHalJsonSupport()
    {
        _mediaTypeCollection.Add(MediaTypeHeaderValue.Parse("application/hal+json"));
        return this;
    }

    public FhydiaBuilder AddJsonLdSupport()
    {
        _mediaTypeCollection.Add(MediaTypeHeaderValue.Parse("application/ld+json"));
        return this;
    }

    public FhydiaBuilder AddCollectionJsonSupport()
    {
        _mediaTypeCollection.Add(MediaTypeHeaderValue.Parse("application/collection+json"));
        return this;
    }

    public FhydiaBuilder Configure(Action<HyperMediaConfigurationBuilder> configure)
    {
        configure.Invoke(_builder);
        return this;
    }

    public FhydiaBuilder Configure(Assembly[] assemblies)
    {
        var genericTypeConfiguration = typeof(ITypeConfiguration<>);
        foreach (var assembly in assemblies)
        {
            var typesToConfigure = assembly
                .GetTypes()
                .Where(type =>
                    type.GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeConfiguration));

            foreach (var typeToConfigure in typesToConfigure)
            {
                // var typeToGenerateInfo = genericTypeConfiguration.MakeGenericType(genericType);

                var typeConfiguration = (ITypeConfiguration)Activator.CreateInstance(typeToConfigure);
                var configureTypeMethodInfo = typeof(HyperMediaConfigurationBuilder).GetMethod(nameof(HyperMediaConfigurationBuilder.ConfigureType));

                var genericInterface = typeToConfigure.GetInterfaces().FirstOrDefault(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeConfiguration);

                var genericType = genericInterface.GetGenericArguments().First();

                var configureTypeMethod = configureTypeMethodInfo.MakeGenericMethod(genericType);
                var typeConfigurationBuilder = (TypeConfigurationBuilder)configureTypeMethod.Invoke(_builder, null);
                typeConfiguration.Configure(typeConfigurationBuilder);
            }
        }

        return this;
    }

    public IServiceCollection Build()
    {
        _serviceCollection.AddHttpContextAccessor();
        _serviceCollection.AddScoped<IHyperMediaJsonEnricher, HyperMediaJsonEnricher>();
        _serviceCollection.AddScoped<IProvideHyperMediaTypeFormatter, HyperMediaTypeFormatterProvider>();

        _serviceCollection.AddControllers(c => c.Filters.Add<HyperMediaResultFilter>());

        _serviceCollection.Configure<MvcOptions>(options =>
        {
            var hyperMediaJsonOutputFormatter = new HyperMediaJsonOutputFormatter(_serializerOptions, _mediaTypeCollection);
            options.OutputFormatters.Insert(0, hyperMediaJsonOutputFormatter);
        });

        _serviceCollection.AddSingleton(_builder.Build());

        return _serviceCollection;
    }
}

public interface ITypeConfiguration
{
    public void Configure(TypeConfigurationBuilder builder);
}

public interface ITypeConfiguration<T> : ITypeConfiguration where T : class, new()
{
    public void Configure(TypeConfigurationBuilder<T> builder);

    void ITypeConfiguration.Configure(TypeConfigurationBuilder builder)
    {
        Configure((TypeConfigurationBuilder<T>)builder);
    }
}