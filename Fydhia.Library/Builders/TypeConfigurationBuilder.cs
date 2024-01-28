using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Fydhia.Library;

public abstract class TypeEnricherBuilder
{
    internal abstract TypeConfiguration Build();
}

public class TypeConfigurationBuilder<TType> : TypeEnricherBuilder where TType : class, new()
{
    public HyperMediaConfigurationBuilder HyperMediaConfigurationBuilder { get; }
    private readonly List<LinkConfigurationBuilder> _linksConfigurationBuilders = new();

    internal TypeConfigurationBuilder(HyperMediaConfigurationBuilder hyperMediaConfigurationBuilder)
    {
        HyperMediaConfigurationBuilder = hyperMediaConfigurationBuilder;
    }

    public LinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TControllerType>(
        string methodName, string? rel = null)
        where TControllerType : Controller
    {
        var linkConfigurationBuilder = new LinkConfigurationBuilder<TType, TControllerType>(this);
        linkConfigurationBuilder.WithMethod(methodName);

        if (!string.IsNullOrWhiteSpace(rel))
            linkConfigurationBuilder.WithRel(rel);

        _linksConfigurationBuilders.Add(linkConfigurationBuilder);

        return linkConfigurationBuilder;
    }

    internal override TypeConfiguration Build()
    {
        var linkConfigurations = _linksConfigurationBuilders.Select(linkBuilder => linkBuilder.Build());
        return new TypeConfiguration(typeof(TType).GetTypeInfo(), linkConfigurations);
    }
}