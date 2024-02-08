using System.Dynamic;
using Fydhia.Core.Common;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Configurations;

public abstract class LinkConfiguration
{
    private readonly IDictionary<string, string> _parameterMappings;
    private readonly IDictionary<string, string> _headerMappings;

    protected readonly LinkOptions LinkOptions = new()
    {
        LowercaseUrls = true,
        LowercaseQueryStrings = true,
        AppendTrailingSlash = false
    };

    private EndpointInfo EndpointInfo { get; }
    public string Rel { get; }
    public string? Title { get; internal init; }
    public string? Name { get; internal init; }
    public bool Templated { get; internal init; }


    public const string SelfLinkRel = "self";

    protected LinkConfiguration(string rel, EndpointInfo endpointInfo, IDictionary<string, string>? parameterMappings = null, IDictionary<string, string>? headerMappings = null)
    {
        Rel = rel.ToSnakeCase();
        EndpointInfo = endpointInfo;
        _parameterMappings = parameterMappings ?? new Dictionary<string, string>();
        _headerMappings = headerMappings ?? new Dictionary<string, string>();
    }

    public string GenerateHref(HttpContext httpContext, LinkGenerator linkGenerator,
        IDictionary<string, object?> responseObjectProperties)
    {
        if (Templated)
        {
            return CreateTemplatedLink(httpContext, EndpointInfo.TemplatePath);
        }

        var routeValues = BuildRouteValues(responseObjectProperties);
        var path = GenerateLink(httpContext, linkGenerator, routeValues);

        return CreateNormalLink(httpContext, path);
    }

    protected abstract string GenerateLink(HttpContext httpContext, LinkGenerator linkGenerator, ExpandoObject routeValues);

    public IHeaderDictionary? GenerateHeaders(HttpContext httpContext, IDictionary<string, object?> responseObjectProperties)
    {
        if (Templated)
        {
            return CreateTemplatedHeaders();
        }

        return BuildHeaderValues(httpContext, responseObjectProperties);
    }

    private IHeaderDictionary? CreateTemplatedHeaders()
    {
        if(!EndpointInfo.Headers.Any())
        {
            return null;
        }

        var result = new HeaderDictionary();
        foreach (var header in EndpointInfo.Headers)
        {
            result.TryAdd(header.BinderModelName, $"{{{header.BinderModelName}}}");
        }

        return result;
    }

    private IHeaderDictionary? BuildHeaderValues(HttpContext httpContext,
        IDictionary<string, object?> responseObjectProperties)
    {
        if (!_headerMappings.Any())
        {
            return null;
        }

        var result = new HeaderDictionary();
        foreach (var headerMapping in _headerMappings)
        {
            if (!responseObjectProperties.TryGetValue(headerMapping.Value, out var value))
            {
                continue;
            }

            result.TryAdd(headerMapping.Key, value?.ToString());
        }

        return result;
    }

    private ExpandoObject BuildRouteValues(IDictionary<string, object?> responseObjectProperties)
    {
        var result = new ExpandoObject();
        foreach (var parameterMapping in _parameterMappings)
        {
            if (!responseObjectProperties.TryGetValue(parameterMapping.Value, out var value))
            {
                continue;
            }

            var parameter =
                EndpointInfo.Parameters.FirstOrDefault(parameter =>
                    parameter.Name == parameterMapping.Key);

            if(parameter is null)
            {
                continue;
            }

            result.TryAdd(parameterMapping.Key, value);
        }

        return result;
    }

    private static string CreateTemplatedLink(HttpContext httpContext, string templatePath)
    {
        return BuildHref(httpContext, templatePath);
    }

    private static string CreateNormalLink(HttpContext httpContext, string path)
    {
        return BuildHref(httpContext, path);
    }

    private static string BuildHref(HttpContext httpContext, string path)
    {
        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{path}";
    }
}