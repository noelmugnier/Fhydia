using Fhydia.Sample;
using Fydhia.Library;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services
    .AddFhydia()
    .AddHalJson();

builder.Services.AddSingleton(provider =>
{
    var hyperMediaConfiguration = new HyperMediaEnricherBuilder(provider.GetRequiredService<LinkGenerator>());
    hyperMediaConfiguration
        .ConfigureEnricherForType<CustomReturnType>()
            .ConfigureControllerLink<TestController>(nameof(TestController.GetFromRouteParam), "self")
                .WithParameterMapping("id", nameof(CustomReturnType.Id))
        .TypeEnricherBuilder
        .HyperMediaEnricherBuilder
        .ConfigureEnricherForType<SubType>()
            .ConfigureControllerLink<TestController>(nameof(TestController.GetFromQueryParam), "self")
                .WithParameterMapping("id", nameof(CustomReturnType.Id));

    return hyperMediaConfiguration.Build();
});

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

app.UseFhydia();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program{ }