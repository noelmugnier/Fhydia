using System.Reflection;

namespace Fydhia.Core.Configurations;

public class TypeConfigurationCollection : List<TypeConfiguration>
{
    public TypeConfigurationCollection(IEnumerable<TypeConfiguration> types)
    {
        AddRange(types);
    }

    public TypeConfiguration? GetConfiguration(Type? typeToConfigure)
    {
        if (typeToConfigure is null)
            return null;

        return this
            .SingleOrDefault(t => t.TypeToConfigure == typeToConfigure.GetTypeInfo());
    }
}