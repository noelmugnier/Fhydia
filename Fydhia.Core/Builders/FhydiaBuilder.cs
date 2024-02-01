using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fydhia.Core.Configurations;
using Fydhia.Core.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Core.Builders;

public class FhydiaBuilder
{
    internal readonly IServiceCollection ServiceCollection;
    internal readonly JsonSerializerOptions SerializerOptions = new ()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    internal readonly MediaTypeCollection MediaTypeCollection = new();
    private readonly HyperMediaConfigurationBuilder _builder;

    internal FhydiaBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
        _builder = new HyperMediaConfigurationBuilder(SerializerOptions, MediaTypeCollection);
    }

    public FhydiaBuilder ConfigureJsonSerializerOptions(Func<JsonSerializerOptions, JsonSerializerOptions> configure)
    {
        configure(SerializerOptions);
        
        //be sure to be consistent between IDictionary (expando or dynamic) and object property naming
        if(SerializerOptions.DictionaryKeyPolicy != SerializerOptions.PropertyNamingPolicy)
            SerializerOptions.DictionaryKeyPolicy = SerializerOptions.PropertyNamingPolicy;

        return this;
    }

    public FhydiaBuilder AddHalFormatter()
    {
        MediaTypeCollection.Add(MediaTypeHeaderValue.Parse(JsonHalTypeFormatter.MediaType));
        return this;
    }

    public FhydiaBuilder Configure(Action<HyperMediaConfigurationBuilder> configure)
    {
        configure.Invoke(_builder);
        return this;
    }

    public FhydiaBuilder Configure(Assembly[] assemblies)
    {
        var genericTypeConfiguration = typeof(ITypeConfigurator<>);
        foreach (var assembly in assemblies)
        {
            var typesToConfigure = assembly
                .GetTypes()
                .Where(type =>
                    type.GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeConfiguration));

            foreach (var typeToConfigure in typesToConfigure)
            {
                var typeConfiguration = (ITypeConfigurator)Activator.CreateInstance(typeToConfigure)!;
                var configureTypeMethodInfo = typeof(HyperMediaConfigurationBuilder).GetMethod(nameof(HyperMediaConfigurationBuilder.ConfigureType));

                var genericInterface = typeToConfigure.GetInterfaces().SingleOrDefault(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeConfiguration);

                var genericType = genericInterface!.GetGenericArguments().First();

                var configureTypeMethod = configureTypeMethodInfo!.MakeGenericMethod(genericType);
                var typeConfigurationBuilder = (TypeConfigurationBuilder)configureTypeMethod.Invoke(_builder, null)!;
                typeConfiguration!.Configure(typeConfigurationBuilder);
            }
        }

        return this;
    }

    internal IServiceCollection Build()
    {
        ServiceCollection.AddHttpContextAccessor();
        ServiceCollection.AddScoped<IHyperMediaJsonEnricher, HyperMediaJsonEnricher>();
        ServiceCollection.AddScoped<IProvideHyperMediaTypeFormatter, HyperMediaTypeFormatterProvider>();

        ServiceCollection.AddSingleton(_builder.Build());

        return ServiceCollection;
    }
}