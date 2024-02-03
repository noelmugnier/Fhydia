using System.Linq.Expressions;
using Fydhia.Core.Builders;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Mvc;

namespace Fhydia.Controllers;

public class ActionLinkConfigurationBuilder<TType, TControllerType> : LinkConfigurationBuilder
    where TControllerType : Controller
    where TType : class, new()
{
    private string? _rel;
    private string _methodName;
    private Dictionary<string, string> _parameterMappings = new();
    private string _name;
    private string _title;
    private bool _templated;

    public TypeConfigurationBuilder<TType> TypeConfigurationBuilder { get; }
    public HyperMediaConfigurationBuilder HyperMediaConfigurationBuilder { get; }

    internal ActionLinkConfigurationBuilder(TypeConfigurationBuilder<TType> typeConfigurationBuilder, string methodName, string? rel = null)
    {
        TypeConfigurationBuilder = typeConfigurationBuilder;
        HyperMediaConfigurationBuilder = typeConfigurationBuilder.HyperMediaConfigurationBuilder;

        WithRel(rel);
        MapToMethod(methodName);
    }

    public ActionLinkConfigurationBuilder<TType, TControllerType> WithRel(string? rel)
    {
        _rel = rel;
        return this;
    }

    public ActionLinkConfigurationBuilder<TType, TControllerType> MapToMethod(string methodName)
    {
        _methodName = methodName;
        return this;
    }

    public ActionLinkConfigurationBuilder<TType, TControllerType> WithName(string name)
    {
        _name = name;
        return this;
    }

    public ActionLinkConfigurationBuilder<TType, TControllerType> WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ActionLinkConfigurationBuilder<TType, TControllerType> AsTemplated(bool templated)
    {
        _templated = templated;
        return this;
    }

    public ActionLinkConfigurationBuilder<TType, TControllerType> WithParameterMapping(Expression<Func<TType, object?>> propertyExpression, string parameterName)
    {
        if(propertyExpression.Body is UnaryExpression unaryExpression)
            _parameterMappings.Add(parameterName, ((MemberExpression)unaryExpression.Operand).Member.Name);
        else if (propertyExpression.Body is MemberExpression memberExpression)
            _parameterMappings.Add(parameterName, memberExpression.Member.Name);
        else
            throw new InvalidOperationException();

        return this;
    }
    
    internal override LinkConfiguration Build()
    {
        var linkConfiguration =
            ActionLinkConfiguration<TControllerType>.Create(_rel, _methodName, _parameterMappings, _name, _title,
                _templated);
        return linkConfiguration;
    }
}