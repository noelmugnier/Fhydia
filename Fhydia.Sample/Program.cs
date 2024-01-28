using Fhydia.Sample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services
    .AddFhydia()
    .Configure(new []{typeof(OtherTypeConfiguration).Assembly})
    .Configure(hyperMediaConfigurationBuilder =>
    {
        hyperMediaConfigurationBuilder
            .ConfigureType<CustomReturnType>()
            .ConfigureSelfLink<TestController>(controller => controller.GetFromQueryParam)
                .WithParameterMapping(type => type.Id, "id")
        .HyperMediaConfigurationBuilder
            .ConfigureType<SubType>()
            .ConfigureSelfLink<TestController>(controller => controller.GetFromQueryParam)
                .WithParameterMapping(type => type.Id, "id")
            .HyperMediaConfigurationBuilder
            .ConfigureType<Other>()
            .ConfigureLink<TestController>(controller => controller.GetFromQueryParam, "test")
            .WithParameterMapping(type => type.Id, "id");
    })
    .AddHalJsonSupport()
    .Build();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

app.UseFhydia();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program{ }