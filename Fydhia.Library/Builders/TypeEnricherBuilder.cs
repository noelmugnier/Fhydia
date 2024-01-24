using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Fydhia.Library;

public abstract class TypeEnricherBuilder
{
    internal abstract TypeEnricherConfiguration Build();
}

public class TypeEnricherBuilder<TType> : TypeEnricherBuilder where TType : class, new()
{
    public HyperMediaConfigurationBuilder HyperMediaConfigurationBuilder { get; }
    private readonly List<LinkConfigurationBuilder> _linksConfigurationBuilders = new();

    internal TypeEnricherBuilder(HyperMediaConfigurationBuilder hyperMediaConfigurationBuilder)
    {
        HyperMediaConfigurationBuilder = hyperMediaConfigurationBuilder;
    }

    public LinkConfigurationBuilder<TType, TControllerType> ConfigureControllerLink<TControllerType>(
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

    internal override TypeEnricherConfiguration Build()
    {
        var linkConfigurations = _linksConfigurationBuilders.Select(linkBuilder => linkBuilder.Build());
        return new TypeEnricherConfiguration(typeof(TType).GetTypeInfo(), linkConfigurations);
    }
}