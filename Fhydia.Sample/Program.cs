using System.Text.Json;
using Fhydia.ControllerActions;
using Fhydia.ControllerActions.Extensions;
using Fhydia.MinimalApi;
using Fhydia.Sample;
using Fydhia.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services
    .AddFhydia(fhydia =>
    {
        fhydia
            .AddMinimalApiSupport()
            .AddControllerSupport()
            .ConfigureJsonSerializerOptions(options =>
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                return options;
            })
            .Configure(new[] { typeof(OtherTypeConfigurator).Assembly })
            .Configure(configurationBuilder =>
            {
                configurationBuilder
                    .ConfigureType<CustomReturnType>()
                    .ConfigureSelfLink<CustomReturnType, TestController>(controller => controller.GetFromQueryParam)
                    .WithParameterMapping(type => type.Id, "id")
                    .HyperMediaConfigurationBuilder
                    .ConfigureType<SubType>()
                    .ConfigureSelfLink<SubType, TestController>(controller => controller.GetFromQueryParam)
                    .WithParameterMapping(type => type.Id, "id")
                    .HyperMediaConfigurationBuilder
                    .ConfigureType<Other>()
                    .ConfigureLink<Other, TestController>(controller => controller.GetFromQueryParam, "test")
                    .WithParameterMapping(type => type.Id, "id");
            })
            .AddHalFormatter();
    });

var app = builder.Build();

app.UseRouting();

app.MapGet("/api/minimal-api/{id}", (int id) => Results.Extensions.HyperMedia(
    new CustomReturnType
    {
        Count = id,
        Inner = new SubType()
        {
            Uber = new CustomReturnType()
        }
    })).WithName("test");

app.MapDefaultControllerRoute();

app.UseFhydia();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

namespace Fhydia.Sample
{
    public partial class Program
    {
    }
}