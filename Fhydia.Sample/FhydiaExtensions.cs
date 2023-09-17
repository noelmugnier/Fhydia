namespace Fhydia.Sample;

public static class FhydiaExtensions
{
    public static IServiceCollection AddFhydia(this IServiceCollection services)
    {
        services.AddControllers(c => c.Filters.Add<HyperMediaResultFilter>());
        services.AddScoped<HyperMediaEnricher>();

        TypeConfig typeConfig = new TypeConfig(typeof(MyModel));
        typeConfig.AddLink("self", "http://localhost");

        services.AddSingleton(new HyperMediaConfiguration(new List<TypeConfig>() { typeConfig }));

        return services;
    }

    public static IApplicationBuilder UseFhydia(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        return app;
    }
}

