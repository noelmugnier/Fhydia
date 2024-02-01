using Fydhia.Core.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fhydia.ControllerActions;

public static class ConfigureControllerActions
{
    public static FhydiaBuilder AddControllerSupport(this FhydiaBuilder fhydiaBuilder)
    {
        MvcServiceCollectionExtensions.AddControllers(fhydiaBuilder.ServiceCollection, c => c.Filters.Add<HyperMediaResultFilter>());

        OptionsServiceCollectionExtensions.Configure<MvcOptions>(fhydiaBuilder.ServiceCollection, options =>
        {
            var hyperMediaJsonOutputFormatter = new HyperMediaJsonOutputFormatter(fhydiaBuilder.SerializerOptions, fhydiaBuilder.MediaTypeCollection);
            options.OutputFormatters.Insert(0, hyperMediaJsonOutputFormatter);
        });
        
        return fhydiaBuilder;
    }
}