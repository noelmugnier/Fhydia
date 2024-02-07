using System.Linq.Expressions;
using Fydhia.Core.Configurations;
using Fydhia.Core.Parser;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Builders;

public class NamedLinkConfigurationBuilder<TType> : LinkConfigurationBuilder where TType : class, new()
{
    private string? _rel;
    private string _endpointName;
    private Dictionary<string, string> _parameterMappings = new();
    private string _name;
    private string _title;
    private bool _templated;

    public TypeConfigurationBuilder<TType> TypeBuilder { get; }
    public HyperMediaConfigurationBuilder HyperMediaConfigurationBuilder { get; }

    internal NamedLinkConfigurationBuilder(TypeConfigurationBuilder<TType> typeConfigurationBuilder, string endpointName, string? rel)
    {
        TypeBuilder = typeConfigurationBuilder;
        HyperMediaConfigurationBuilder = typeConfigurationBuilder.HyperMediaConfigurationBuilder;

        WithRel(rel);
        MapToEndpoint(endpointName);
    }

    public NamedLinkConfigurationBuilder<TType> WithRel(string? rel)
    {
        _rel = rel;
        return this;
    }

    public NamedLinkConfigurationBuilder<TType> MapToEndpoint(string endpointName)
    {
        _endpointName = endpointName;
        return this;
    }

    public NamedLinkConfigurationBuilder<TType> WithName(string name)
    {
        _name = name;
        return this;
    }

    public NamedLinkConfigurationBuilder<TType> WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public NamedLinkConfigurationBuilder<TType> AsTemplated(bool templated)
    {
        _templated = templated;
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
    
    internal override LinkConfiguration Build(EndpointDataSource endpointDataSource)
    {
        if (string.IsNullOrWhiteSpace(_endpointName))
        {
            throw new ArgumentException($"Endpoint name must be provided to build a link configuration");
        }

        var routeEndpoint = GetRouteEndpoint(endpointDataSource);
        if (routeEndpoint == null)
        {
            throw new InvalidOperationException($"Endpoint with name {_endpointName} not found");
        }

        var routeEndpointParser = new RouteEndpointParser();

        return new NamedLinkConfiguration(_rel, _endpointName, _parameterMappings)
        {
            Name = _name,
            Title = _title,
            Templated = _templated,
            ReturnType = routeEndpointParser.GetReturnedType(routeEndpoint),
            Parameters = routeEndpointParser.GetParameters(routeEndpoint),
            HttpMethod = routeEndpointParser.GetHttpMethod(routeEndpoint),
            TemplatePath = routeEndpoint.RoutePattern.RawText
        };
    }

    private RouteEndpoint? GetRouteEndpoint(EndpointDataSource routeBuilder)
    {
        foreach (var endpoint in routeBuilder.Endpoints)
        {
            var endpointNameMetadata = endpoint.Metadata.GetMetadata<EndpointNameMetadata>();
            var endpointNameAttribute = endpoint.Metadata.GetMetadata<EndpointNameAttribute>();
            if (endpointNameMetadata == null && endpointNameAttribute == null)
                continue;

            if (endpointNameMetadata?.EndpointName != _endpointName &&
                endpointNameAttribute?.EndpointName != _endpointName)
                continue;

            return endpoint as RouteEndpoint;
        }

        return null;
    }
}