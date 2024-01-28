using Fydhia.Library;

namespace Fhydia.Sample;

public class OtherTypeConfigurator : ITypeConfigurator<Other>
{
    public void Configure(TypeConfigurationBuilder<Other> builder)
    {
        builder.ConfigureSelfLink<TestController>(controller => controller.GetFromHeaderParams)
            .WithParameterMapping(type => type.Id, "id");

    }
}