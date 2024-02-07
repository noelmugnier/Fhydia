using System.Reflection;
using Fydhia.Core.Configurations;
using Fydhia.Core.Enrichers;
using Fydhia.Core.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Fydhia.Core.Builders;

public class FhydiaBuilder
{
    internal readonly IServiceCollection ServiceCollection;
    
    internal readonly MediaTypeCollection MediaTypeCollection = new();
    private readonly HyperMediaConfigurationBuilder _builder;

    internal FhydiaBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
        _builder = new HyperMediaConfigurationBuilder(MediaTypeCollection);
    }

    public FhydiaBuilder AddHalFormatter()
    {
        MediaTypeCollection.Add(MediaTypeHeaderValue.Parse(JsonHalTypesFormatter.MediaType));
        return this;
    }

    public FhydiaBuilder Configure(Action<HyperMediaConfigurationBuilder> configure)
    {
        configure.Invoke(_builder);
        return this;
    }

    public FhydiaBuilder Configure(Assembly[] assemblies)
    {
        var genericTypeConfiguration = typeof(IHyperMediaTypeConfigurator<>);
        foreach (var assembly in assemblies)
        {
            var typesToConfigure = assembly
                .GetTypes()
                .Where(type =>
                    type.GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeConfiguration));

            foreach (var typeToConfigure in typesToConfigure)
            {
                var typeConfiguration = (IHyperMediaTypeConfigurator)Activator.CreateInstance(typeToConfigure)!;
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
        ServiceCollection.AddScoped<IHyperMediaObjectEnricher, HyperMediaObjectEnricher>();
        ServiceCollection.AddScoped<IProvideHyperMediaTypesFormatter, HyperMediaTypesFormatterProvider>();

        ServiceCollection.AddSingleton<HyperMediaConfiguration>(provider => _builder.Build(provider.GetRequiredService<EndpointDataSource>()));

        return ServiceCollection;
    }
}