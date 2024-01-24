using Fhydia.Sample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services
    .AddFhydia()
    .AddHalJsonSupport()
    .ConfigureEnricher(enricher =>
    {
        enricher
            .ConfigureForType<CustomReturnType>()
            .ConfigureControllerLink<TestController>(nameof(TestController.GetFromRouteParam), "self")
                .WithParameterMapping("id", nameof(CustomReturnType.Id))
            .TypeEnricherBuilder
            .HyperMediaConfigurationBuilder
            .ConfigureForType<SubType>()
            .ConfigureControllerLink<TestController>(nameof(TestController.GetFromQueryParam), "self")
                .WithParameterMapping("id", nameof(CustomReturnType.Id));
    });

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

app.UseFhydia();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program{ }