using Fydhia.Core.Builders;

namespace Fydhia.Core.Configurations;

public interface IHyperMediaTypeConfigurator
{
    public void Configure(TypeConfigurationBuilder builder);
}

public interface IHyperMediaTypeConfigurator<T> : IHyperMediaTypeConfigurator where T : class, new()
{
    public void Configure(TypeConfigurationBuilder<T> builder);

    void IHyperMediaTypeConfigurator.Configure(TypeConfigurationBuilder builder)
    {
        Configure((TypeConfigurationBuilder<T>)builder);
    }
}