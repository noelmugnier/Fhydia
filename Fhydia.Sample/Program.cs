using System.Text.Json;
using System.Text.Json.Serialization;
using Fhydia.Sample;
using Fhydia.Sample.Configurations;
using Fhydia.Sample.Controllers;
using Fydhia.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(opt =>
{
    opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opt.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    opt.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services
    .AddFhydia(fhydia =>
    {
        fhydia
            .Configure(new[] { typeof(OtherTypeConfigurator).Assembly })
            .Configure(configurationBuilder =>
            {
                configurationBuilder
                    .ConfigureType<CustomReturnType>()
                    .ConfigureSelfLink<TestController>(controller => controller.GetFromQueryParam)
                    .WithParameterMapping(type => type.Id, "id")
                    .HyperMediaConfigurationBuilder
                    .ConfigureType<SubType>()
                    .ConfigureSelfLink<TestController>(controller => controller.GetFromRouteParam)
                    .WithParameterMapping(type => type.Id, "id")
                    .HyperMediaConfigurationBuilder
                    .ConfigureType<Other>()
                    .ConfigureLink("SuperTest", "test")
                    .WithParameterMapping(type => type.Id, "id")
                    .TypeConfigurationBuilder
                    .ConfigureLink(nameof(DefaultHandler.Test), "super")
                    .AsTemplated(true)
                    .WithParameterMapping(type => type.Id, "id");
            })
            .AddHalFormatter();
    });

var app = builder.Build();

app.UseRouting();

var router = app.UseFhydia();

router.MapControllers();

router.MapGet("/api/minimal-api/{test}", DefaultHandler.Test)
    .WithName(nameof(DefaultHandler.Test));

router.MapGet("/api/minimal-api/test", () => 
    TypedResults.Ok(new CustomReturnType
        {
            Count = 5,
            Inner = new SubType()
            {
                Uber = new CustomReturnType()
            }
        }))
    .WithName("sample");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

namespace Fhydia.Sample
{
    public partial class Program
    {
    }

    class DefaultHandler
    {
        public static Ok<CustomReturnType> Test(HttpContext context, [FromServices] IConfiguration config, [FromRoute(Name = "test")] int id)
        {
            return TypedResults.Ok(new CustomReturnType
                {
                    Count = id,
                    Inner = new SubType()
                    {
                        Uber = new CustomReturnType()
                    }
                });
        }
    }
}