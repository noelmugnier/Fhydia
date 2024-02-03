using System.Linq.Expressions;
using Fydhia.Core.Configurations;

namespace Fydhia.Core.Builders;

public class NamedLinkConfigurationBuilder<TType> : LinkConfigurationBuilder where TType : class, new()
{
    private string? _rel;
    private string _name;
    private Dictionary<string, string> _parameterMappings = new();

    public TypeConfigurationBuilder<TType> TypeConfigurationBuilder { get; }
    public HyperMediaConfigurationBuilder HyperMediaConfigurationBuilder { get; }

    internal NamedLinkConfigurationBuilder(TypeConfigurationBuilder<TType> typeConfigurationBuilder)
    {
        TypeConfigurationBuilder = typeConfigurationBuilder;
        HyperMediaConfigurationBuilder = typeConfigurationBuilder.HyperMediaConfigurationBuilder;
    }

    public NamedLinkConfigurationBuilder<TType> WithRel(string? rel)
    {
        _rel = rel;
        return this;
    }

    public NamedLinkConfigurationBuilder<TType> WithName(string name)
    {
        _name = name;
        return this;
    }

    public NamedLinkConfigurationBuilder<TType> WithParameterMapping(Expression<Func<TType, object?>> propertyExpression, string parameterName)
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
        var linkConfiguration = NamedLinkConfiguration.Create(_rel, _name, _parameterMappings);
        return linkConfiguration;
    }
}