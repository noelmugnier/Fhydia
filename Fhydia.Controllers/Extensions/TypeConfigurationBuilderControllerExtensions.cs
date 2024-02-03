using System.Linq.Expressions;
using System.Reflection;
using Fydhia.Core.Builders;
using Microsoft.AspNetCore.Mvc;

namespace Fhydia.Controllers.Extensions;

public static class TypeConfigurationBuilderControllerExtensions
{
    public static ActionLinkConfigurationBuilder<TType, TControllerType> ConfigureSelfLink<TType, TControllerType>(this TypeConfigurationBuilder<TType> typeBuilder, 
        Expression<Func<TControllerType, Delegate?>> methodExpression)
        where TControllerType : Controller where TType : class, new()
    {
        return typeBuilder.ConfigureLink(methodExpression, "self");
    }

    public static ActionLinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TType, TControllerType>(this TypeConfigurationBuilder<TType> typeBuilder, 
        Expression<Func<TControllerType, Delegate?>> methodExpression, string? rel = null)
        where TControllerType : Controller where TType : class, new()
    {
        var methodName =
            ((MethodInfo)((ConstantExpression)((MethodCallExpression)((UnaryExpression)methodExpression.Body).Operand)
                .Object).Value).Name;
        return typeBuilder.ConfigureLink<TType, TControllerType>(methodName, rel);
    }

    private static ActionLinkConfigurationBuilder<TType, TControllerType> ConfigureLink<TType, TControllerType>(this TypeConfigurationBuilder<TType> typeBuilder, 
        string methodName, string? rel = null)
        where TControllerType : Controller where TType : class, new()
    {
        var linkConfigurationBuilder = new ActionLinkConfigurationBuilder<TType, TControllerType>(typeBuilder, methodName, rel);

        typeBuilder.AddLinkBuilder(linkConfigurationBuilder);
        return linkConfigurationBuilder;
    }
}