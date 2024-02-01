using Fydhia.Core.Builders;

namespace Fydhia.Core.Configurations;

public interface ITypeConfigurator
{
    public void Configure(TypeConfigurationBuilder builder);
}

public interface ITypeConfigurator<T> : ITypeConfigurator where T : class, new()
{
    public void Configure(TypeConfigurationBuilder<T> builder);

    void ITypeConfigurator.Configure(TypeConfigurationBuilder builder)
    {
        Configure((TypeConfigurationBuilder<T>)builder);
    }
}