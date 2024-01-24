using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static FhydiaBuilder AddFhydia(this IServiceCollection services)
    {
        return AddFhydia(services, (options => options));
    }

    public static FhydiaBuilder AddFhydia(this IServiceCollection services, Func<JsonSerializerOptions, JsonSerializerOptions> configureSerializer)
    {
        return new FhydiaBuilder(services, configureSerializer(new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    public static IApplicationBuilder UseFhydia(this IApplicationBuilder app)
    {
        return app;
    }
}