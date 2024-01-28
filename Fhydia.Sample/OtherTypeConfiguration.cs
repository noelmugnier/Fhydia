using Fydhia.Library;

namespace Fhydia.Sample;

public class OtherTypeConfiguration : ITypeConfiguration<Other>
{
    public void Configure(TypeConfigurationBuilder<Other> builder)
    {
        builder.ConfigureSelfLink<TestController>(controller => controller.GetFromHeaderParams)
            .WithParameterMapping(type => type.Id, "id");

    }
}