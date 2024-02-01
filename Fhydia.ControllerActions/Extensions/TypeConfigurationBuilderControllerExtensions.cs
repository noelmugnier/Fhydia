using System.Linq.Expressions;
using System.Reflection;
using Fhydia.ControllerActions.ControllerLink;
using Fydhia.Core.Builders;
using Microsoft.AspNetCore.Mvc;

namespace Fhydia.ControllerActions.Extensions;

public static class TypeConfigurationBuilderControllerExtensions
{
    public static ControllerLinkConfigurationBuilder<TType, TControllerType> ConfigureSelfLink<TType, TControllerType>(this TypeConfigurationBuilder<TType> typeBuilder, 
        Expression<Func<TControllerType, Delegate?>> methodExpression)
        where TControllerType : Controller where TType : class, new()
    {
        return typeBuilder.ConfigureLink<TType, TControllerType>(methodExpression, "self");
    }

    public static ControllerLinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TType, TControllerType>(this TypeConfigurationBuilder<TType> typeBuilder, 
        Expression<Func<TControllerType, Delegate?>> methodExpression, string? rel = null)
        where TControllerType : Controller where TType : class, new()
    {
        var methodName =
            ((MethodInfo)((ConstantExpression)((MethodCallExpression)((UnaryExpression)methodExpression.Body).Operand)
                .Object).Value).Name;
        return typeBuilder.ConfigureLink<TType, TControllerType>(methodName, rel);
    }

    private static ControllerLinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TType, TControllerType>(this TypeConfigurationBuilder<TType> typeBuilder, 
        string methodName, string? rel = null)
        where TControllerType : Controller where TType : class, new()
    {
        var linkConfigurationBuilder = new ControllerLinkConfigurationBuilder<TType, TControllerType>(typeBuilder);
        linkConfigurationBuilder.WithMethod(methodName);

        if (!string.IsNullOrWhiteSpace(rel))
            linkConfigurationBuilder.WithRel(rel);

        typeBuilder.AddLinkBuilder(linkConfigurationBuilder);
        return linkConfigurationBuilder;
    }
}