using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Fydhia.Library;

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

    public ControllerLinkConfigurationBuilder<TType, TControllerType> ConfigureSelfLink<TControllerType>(
        Expression<Func<TControllerType, Delegate?>> methodExpression)
        where TControllerType : Controller
    {
        return ConfigureLink(methodExpression, "self");
    }

    public ControllerLinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TControllerType>(
        Expression<Func<TControllerType, Delegate?>> methodExpression, string? rel = null)
        where TControllerType : Controller
    {
        var methodName =((MethodInfo)((ConstantExpression)((MethodCallExpression)((UnaryExpression)methodExpression.Body).Operand).Object).Value).Name;
        return ConfigureLink<TControllerType>(methodName, rel);
    }

    private ControllerLinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TControllerType>(
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

    internal override TypeConfiguration Build()
    {
        var linkConfigurations = _linksConfigurationBuilders.Select(linkBuilder => linkBuilder.Build());
        return new TypeConfiguration(TypeToConfigure.GetTypeInfo(), linkConfigurations);
    }

    internal override Type GetTypeToConfigure()
    {
        return TypeToConfigure;
    }
}