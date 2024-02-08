using System.Linq.Expressions;
using System.Reflection;
using Fydhia.Core.Common;
using Fydhia.Core.Configurations;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Builders;

public class ActionLinkConfigurationBuilder<TType, TControllerType> : LinkConfigurationBuilder
    where TControllerType : Controller
    where TType : class, new()
{
    private readonly TypeInfo _controllerType = typeof(TControllerType).GetTypeInfo();
    private readonly Dictionary<string, string> _parameterMappings = new();
    private readonly Dictionary<string, string> _headerMappings = new();

    private string? _rel;
    private string _methodName = default!;
    private string _name = default!;
    private string _title = default!;
    private bool _templated;

    private string ControllerName => _controllerType.GetControllerClassName();

    public TypeConfigurationBuilder<TType> TypeBuilder { get; }
    public HyperMediaConfigurationBuilder HyperMediaBuilder { get; }

    internal ActionLinkConfigurationBuilder(TypeConfigurationBuilder<TType> typeConfigurationBuilder, string methodName, string? rel = null)
    {
        TypeBuilder = typeConfigurationBuilder;
        HyperMediaBuilder = typeConfigurationBuilder.HyperMediaConfigurationBuilder;

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

    public ActionLinkConfigurationBuilder<TType, TControllerType> WithHeaderMapping(Expression<Func<TType, object?>> propertyExpression, string headerName)
    {
        if(propertyExpression.Body is UnaryExpression unaryExpression)
            _headerMappings.Add(headerName, ((MemberExpression)unaryExpression.Operand).Member.Name);
        else if (propertyExpression.Body is MemberExpression memberExpression)
            _headerMappings.Add(headerName, memberExpression.Member.Name);
        else
            throw new InvalidOperationException();
        return this;
    }
    
    internal override LinkConfiguration Build(EndpointDataSource endpointDataSource)
    {
        if (string.IsNullOrWhiteSpace(_methodName))
        {
            throw new ArgumentException($"Method name must be provided to build a link configuration for controller {typeof(TControllerType).FullName}");
        }

        var method = _controllerType.GetMethod(_methodName);
        if(method is null)
        {
            throw new ArgumentException($"Method {_methodName} not found on controller {typeof(TControllerType).FullName}");
        }

        var routeEndpoint = GetRouteEndpoint(endpointDataSource);
        if(routeEndpoint is null)
        {
            throw new InvalidOperationException($"Endpoint with method {_methodName} on controller {ControllerName} not found");
        }

        var parsedEndpoint = new EndpointInfo(routeEndpoint);

        return new ActionLinkConfiguration<TControllerType>(_rel, parsedEndpoint, _methodName, ControllerName, _parameterMappings, _headerMappings)
            {
                Name = _name,
                Title = _title,
                Templated = _templated
            };
    }

    private RouteEndpoint? GetRouteEndpoint(EndpointDataSource routeBuilder)
    {
        var routeEndpoint = routeBuilder.Endpoints.FirstOrDefault(endpoint =>
            endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.ControllerName == ControllerName
            && endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.MethodInfo.Name == _methodName) as RouteEndpoint;

        return routeEndpoint;
    }
}