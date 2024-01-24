using System.Reflection;

namespace Fydhia.Library;

public class TypeEnricherConfiguration
{
    internal TypeEnricherConfiguration(TypeInfo typeToEnrich, IEnumerable<LinkConfiguration> configuredLinks)
    {
        TypeToEnrich = typeToEnrich;
        ConfiguredLinks = configuredLinks;
    }

    public TypeInfo TypeToEnrich { get; }
    public IEnumerable<LinkConfiguration> ConfiguredLinks { get; }
}