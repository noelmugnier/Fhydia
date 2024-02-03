using Fydhia.Core.Builders;
using Fydhia.Core.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

    public static IEndpointRouteBuilder UseFhydia(this IEndpointRouteBuilder app, string? endpointsBasePath = null)
    {
        return app.MapGroup(endpointsBasePath ?? string.Empty)
            .AddEndpointFilter<HyperMediaEndpointFilter>();
    }
}