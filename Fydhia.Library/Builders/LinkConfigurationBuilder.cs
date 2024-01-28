using Microsoft.AspNetCore.Mvc;

namespace Fydhia.Library;

public abstract class LinkConfigurationBuilder
{
    internal abstract LinkConfiguration Build();
}

public class LinkConfigurationBuilder<TType, TControllerType> : LinkConfigurationBuilder
    where TControllerType : Controller
    where TType : class, new()
{
    private string _rel;
    private string _methodName;
    private Dictionary<string, string> _parameterMappings = new();

    public TypeConfigurationBuilder<TType> TypeConfigurationBuilder { get; }

    internal LinkConfigurationBuilder(TypeConfigurationBuilder<TType> typeConfigurationBuilder)
    {
        TypeConfigurationBuilder = typeConfigurationBuilder;
    }

    public LinkConfigurationBuilder<TType, TControllerType> WithRel(string? rel)
    {
        _rel = rel;
        return this;
    }

    public LinkConfigurationBuilder<TType, TControllerType> WithMethod(string methodName)
    {
        _methodName = methodName;
        return this;
    }

    public LinkConfigurationBuilder<TType, TControllerType> WithParameterMapping(string parameterName,
        string propertyName)
    {
        _parameterMappings.Add(parameterName, propertyName);
        return this;
    }

    public LinkConfigurationBuilder<TType, TControllerType> WithParameterMappings(
        IDictionary<string, string> parameterMappings)
    {
        foreach (var parameterMapping in parameterMappings)
        {
            if (_parameterMappings.ContainsKey(parameterMapping.Key))
                _parameterMappings.Remove(parameterMapping.Key);

            _parameterMappings.Add(parameterMapping.Key, parameterMapping.Value);
        }

        return this;
    }

    internal override LinkConfiguration Build()
    {
        var linkConfiguration = LinkConfiguration<TControllerType>.Create(_methodName, _parameterMappings, _rel);
        linkConfiguration.ValidateParameterMappings();

        return linkConfiguration;
    }
}