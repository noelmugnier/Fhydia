using Fhydia.Sample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services
    .AddFhydia()
    .Configure(configBuilder =>
    {
        configBuilder
            .ConfigureType<CustomReturnType>()
            .ConfigureLink<TestController>(nameof(TestController.GetFromRouteParam), "self")
                .WithParameterMapping("id", nameof(CustomReturnType.Id))
            .TypeConfigurationBuilder
            .HyperMediaConfigurationBuilder
            .ConfigureType<SubType>()
            .ConfigureLink<TestController>(nameof(TestController.GetFromQueryParam), "self")
                .WithParameterMapping("id", nameof(CustomReturnType.Id));
    })
    .AddHalJsonSupport();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

app.UseFhydia();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program{ }