using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace Fydhia.Library;

public abstract class LinkConfigurationBuilder
{
    internal abstract LinkConfiguration Build();
}

public class ControllerLinkConfigurationBuilder<TType, TControllerType> : LinkConfigurationBuilder
    where TControllerType : Controller
    where TType : class, new()
{
    private string _rel;
    private string _methodName;
    private Dictionary<string, string> _parameterMappings = new();

    public TypeConfigurationBuilder<TType> TypeConfigurationBuilder { get; }
    public HyperMediaConfigurationBuilder HyperMediaConfigurationBuilder { get; }

    internal ControllerLinkConfigurationBuilder(TypeConfigurationBuilder<TType> typeConfigurationBuilder)
    {
        TypeConfigurationBuilder = typeConfigurationBuilder;
        HyperMediaConfigurationBuilder = typeConfigurationBuilder.HyperMediaConfigurationBuilder;
    }

    public ControllerLinkConfigurationBuilder<TType, TControllerType> WithRel(string? rel)
    {
        _rel = rel;
        return this;
    }

    public ControllerLinkConfigurationBuilder<TType, TControllerType> WithMethod(string methodName)
    {
        _methodName = methodName;
        return this;
    }

    public ControllerLinkConfigurationBuilder<TType, TControllerType> WithParameterMapping(Expression<Func<TType, object?>> propertyExpression, string parameterName)
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
        var linkConfiguration = ControllerLinkConfiguration<TControllerType>.Create(_methodName, _parameterMappings, _rel);
        linkConfiguration.ValidateParameterMappings();

        return linkConfiguration;
    }
}