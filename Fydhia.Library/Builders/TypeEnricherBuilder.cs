using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class TypeEnricherBuilder<TType> : TypeEnricherBuilder where TType : class, new()
{
    public HyperMediaEnricherBuilder HyperMediaEnricherBuilder { get; }
    private readonly LinkGenerator _linkGenerator;
    private readonly List<LinkConfigurationBuilder> _linksConfigurationBuilders = new();

    internal TypeEnricherBuilder(HyperMediaEnricherBuilder hyperMediaEnricherBuilder, LinkGenerator linkGenerator)
    {
        HyperMediaEnricherBuilder = hyperMediaEnricherBuilder;
        _linkGenerator = linkGenerator;
    }

    public ControllerLinkConfigurationBuilder<TType, TControllerType> ConfigureControllerLink<TControllerType>(
        string methodName, string? rel = null)
        where TControllerType : Controller
    {
        var linkConfigurationBuilder = new ControllerLinkConfigurationBuilder<TType, TControllerType>(this);
        linkConfigurationBuilder.WithMethod(methodName);

        if (!string.IsNullOrWhiteSpace(rel))
            linkConfigurationBuilder.WithRel(rel);

        _linksConfigurationBuilders.Add(linkConfigurationBuilder);

        return linkConfigurationBuilder;
    }

    internal override TypeEnricher Build()
    {
        var linkConfigurations = _linksConfigurationBuilders.Select(linkBuilder => linkBuilder.Build());
        return new TypeEnricher(_linkGenerator, typeof(TType).GetTypeInfo(), linkConfigurations);
    }
}

public abstract class TypeEnricherBuilder
{
    internal abstract TypeEnricher Build();
}