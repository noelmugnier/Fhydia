using Fydhia.Library;

namespace Microsoft.Extensions.DependencyInjection;

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