using System.Linq.Expressions;
using System.Reflection;
using Fhydia.Controllers;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Builders;

public abstract class TypeConfigurationBuilder
{
    internal abstract TypeConfiguration Build(EndpointDataSource endpointDataSource);
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

    public NamedLinkConfigurationBuilder<TType> ConfigureSelfLink(string endpointName)
    {
        return ConfigureLink(endpointName, LinkConfiguration.SelfLinkRel);
    }

    public NamedLinkConfigurationBuilder<TType> ConfigureLink(string endpointName, string? rel = null)
    {
        var namedLinkConfigurationBuilder = new NamedLinkConfigurationBuilder<TType>(this, endpointName, rel);
        _linksConfigurationBuilders.Add(namedLinkConfigurationBuilder);

        return namedLinkConfigurationBuilder;
    }

    public ActionLinkConfigurationBuilder<TType, TControllerType> ConfigureSelfLink<TControllerType>(
        Expression<Func<TControllerType, Delegate?>> methodExpression)
        where TControllerType : Controller
    {
        return ConfigureLink(methodExpression, LinkConfiguration.SelfLinkRel);
    }

    public ActionLinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TControllerType>(
        Expression<Func<TControllerType, Delegate?>> methodExpression, string? rel = null)
        where TControllerType : Controller
    {
        var unaryExpression = (UnaryExpression)methodExpression.Body;
        var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
        var constantExpression = (ConstantExpression)methodCallExpression.Object!;
        var methodInfo = (MethodInfo)constantExpression.Value!;

        return ConfigureLink<TControllerType>(methodInfo.Name, rel);
    }
    
    internal override TypeConfiguration Build(EndpointDataSource endpointDataSource)
    {
        var linkConfigurations = _linksConfigurationBuilders.Select(linkBuilder => linkBuilder.Build(endpointDataSource));
        return new TypeConfiguration(TypeToConfigure.GetTypeInfo(), linkConfigurations);
    }

    internal override Type GetTypeToConfigure()
    {
        return TypeToConfigure;
    }

    private ActionLinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TControllerType>(string methodName, string? rel = null)
        where TControllerType : Controller
    {
        var linkConfigurationBuilder = new ActionLinkConfigurationBuilder<TType, TControllerType>(this, methodName, rel);

        _linksConfigurationBuilders.Add(linkConfigurationBuilder);
        return linkConfigurationBuilder;
    }
}