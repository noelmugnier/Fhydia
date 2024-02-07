using System.Linq.Expressions;
using System.Reflection;
using Fydhia.Core.Configurations;
using Fydhia.Core.Extensions;
using Fydhia.Core.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Builders;

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

    private TypeInfo _controllerType = typeof(TControllerType).GetTypeInfo();
    private string _controllerName => _controllerType.GetControllerClassName();

    public TypeConfigurationBuilder<TType> TypeConfigurationBuilder { get; }
    public HyperMediaConfigurationBuilder HyperMediaBuilder { get; }

    internal ActionLinkConfigurationBuilder(TypeConfigurationBuilder<TType> typeConfigurationBuilder, string methodName, string? rel = null)
    {
        TypeConfigurationBuilder = typeConfigurationBuilder;
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
            throw new InvalidOperationException($"Endpoint with method {_methodName} on controller {_controllerName} not found");
        }

        var routeEndpointParser = new RouteEndpointParser();

        return new ActionLinkConfiguration<TControllerType>(_rel, _methodName, _controllerName, _parameterMappings)
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
        var routeEndpoint = routeBuilder.Endpoints.FirstOrDefault(endpoint =>
            endpoint.Metadata.GetMetadata<ControllerActionDescriptor>().ControllerName == _controllerName
            && endpoint.Metadata.GetMetadata<ControllerActionDescriptor>().MethodInfo.Name == _methodName) as RouteEndpoint;

        return routeEndpoint;
    }
}