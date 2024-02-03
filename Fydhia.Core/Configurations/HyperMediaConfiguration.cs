using Microsoft.AspNetCore.Mvc.Formatters;

namespace Fydhia.Core.Configurations;

public class HyperMediaConfiguration
{
    public TypeConfigurationCollection ConfiguredTypes { get; }
    public MediaTypeCollection SupportedMediaTypes { get; }

    internal HyperMediaConfiguration(TypeConfigurationCollection configuredTypes, MediaTypeCollection supportedMediaTypes)
    {
        ConfiguredTypes = configuredTypes;
        SupportedMediaTypes = supportedMediaTypes;
    }
}