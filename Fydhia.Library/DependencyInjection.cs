using Fydhia.Library;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddFhydia(this IServiceCollection services)
    {
        services.AddControllers(c => c.Filters.Add<HyperMediaResultFilter>());

        return services;
    }

    public static IApplicationBuilder UseFhydia(this IApplicationBuilder app)
    {
        return app;
    }
}