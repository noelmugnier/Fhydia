using Fhydia.ControllerActions.Extensions;
using Fydhia.Core.Builders;
using Fydhia.Core.Configurations;

namespace Fhydia.Sample;

public class OtherTypeConfigurator : ITypeConfigurator<Other>
{
    public void Configure(TypeConfigurationBuilder<Other> builder)
    {
        builder.ConfigureSelfLink<Other, TestController>(controller => controller.GetFromHeaderParams)
            .WithParameterMapping(type => type.Id, "id");
    }
}