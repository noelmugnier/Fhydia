using Fydhia.Core.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fydhia.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddFhydia(this IServiceCollection services, Action<FhydiaBuilder> configure)
    {
        var builder = new FhydiaBuilder(services);
        configure(builder);
        return builder.Build();
    }

    public static IApplicationBuilder UseFhydia(this IApplicationBuilder app)
    {
        var builder = app.ApplicationServices.GetRequiredService<IEnumerable<EndpointDataSource>>();
        
        return app;
    }
}