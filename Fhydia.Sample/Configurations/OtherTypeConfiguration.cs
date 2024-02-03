using Fhydia.Controllers.Extensions;
using Fhydia.Sample.Controllers;
using Fydhia.Core.Builders;
using Fydhia.Core.Configurations;

namespace Fhydia.Sample.Configurations;

public class OtherTypeConfigurator : ITypeConfigurator<Other>
{
    public void Configure(TypeConfigurationBuilder<Other> builder)
    {
        builder.ConfigureSelfLink<Other, TestController>(controller => controller.GetFromHeaderParams)
            .WithParameterMapping(type => type.Id, "id");
    }
}