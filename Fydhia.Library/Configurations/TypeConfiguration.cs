using System.Reflection;

namespace Fydhia.Library;

public class TypeConfiguration
{
    internal TypeConfiguration(TypeInfo typeToConfigure, IEnumerable<LinkConfiguration> configuredLinks)
    {
        TypeToConfigure = typeToConfigure;
        ConfiguredLinks = configuredLinks;
    }

    public TypeInfo TypeToConfigure { get; }
    public IEnumerable<LinkConfiguration> ConfiguredLinks { get; }
}