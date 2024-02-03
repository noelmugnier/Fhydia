using System.Reflection;
using Fydhia.Core.Configurations;

namespace Fydhia.Core.Builders;

public abstract class TypeConfigurationBuilder
{
    internal abstract TypeConfiguration Build();
    internal abstract Type GetTypeToConfigure();
}

public class TypeConfigurationBuilder<TType> : TypeConfigurationBuilder where TType : class, new()
{
    public HyperMediaConfigurationBuilder HyperMediaConfigurationBuilder { get; }
    private readonly List<LinkConfigurationBuilder> _linksConfigurationBuilders = new();

    private Type TypeToConfigure => typeof(TType);

    internal TypeConfigurationBuilder(HyperMediaConfigurationBuilder hyperMediaConfigurationBuilder)
    {
        HyperMediaConfigurationBuilder = hyperMediaConfigurationBuilder;
    }

    public NamedLinkConfigurationBuilder<TType> ConfigureSelfLink(string name)
    {
        return ConfigureLink(name, "self");
    }

    public NamedLinkConfigurationBuilder<TType> ConfigureLink(string name, string? rel = null)
    {
        var namedLinkConfigurationBuilder = new NamedLinkConfigurationBuilder<TType>(this);
        _linksConfigurationBuilders.Add(namedLinkConfigurationBuilder);

        namedLinkConfigurationBuilder.WithName(name);
        namedLinkConfigurationBuilder.WithRel(rel);

        return namedLinkConfigurationBuilder;
    }
    
    internal override TypeConfiguration Build()
    {
        var linkConfigurations = _linksConfigurationBuilders.Select(linkBuilder => linkBuilder.Build());
        return new TypeConfiguration(TypeToConfigure.GetTypeInfo(), linkConfigurations);
    }

    internal override Type GetTypeToConfigure()
    {
        return TypeToConfigure;
    }

    internal void AddLinkBuilder(LinkConfigurationBuilder linkConfigurationBuilder)
    {
        _linksConfigurationBuilders.Add(linkConfigurationBuilder);
    }
}