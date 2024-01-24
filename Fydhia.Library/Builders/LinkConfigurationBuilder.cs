using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

    public TypeEnricherBuilder<TType> TypeEnricherBuilder { get; }

    internal LinkConfigurationBuilder(TypeEnricherBuilder<TType> typeEnricherBuilder)
    {
        TypeEnricherBuilder = typeEnricherBuilder;
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
        if (string.IsNullOrWhiteSpace(_methodName))
            throw new ArgumentException(
                $"Method name must be provided to build a link configuration for controller {typeof(TControllerType).FullName}");

        var linkConfiguration = LinkConfiguration.CreateForController<TControllerType>(_methodName, _parameterMappings, _rel);
        var parametersCount = linkConfiguration.Parameters.Count(p =>
            p.BindingSource == BindingSource.Path
            || p.BindingSource == BindingSource.Query
            || p.BindingSource == BindingSource.Header);

        if (parametersCount != _parameterMappings.Count())
            throw new ArgumentException(
                $"Parameter mappings must be provided for all path, query and headers parameters for method {_methodName} on controller {typeof(TControllerType).FullName}");

        return linkConfiguration;
    }
}