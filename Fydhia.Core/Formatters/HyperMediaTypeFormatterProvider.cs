using System.Dynamic;
using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Formatters;

public interface IHyperMediaTypeFormatter
{
    public string FormattedMediaType { get; }
    public void Format(ExpandoObject responseObject, HttpContext httpContext);
}

public interface IProvideHyperMediaTypeFormatter
{
    IHyperMediaTypeFormatter GetFormatter(IEnumerable<string> acceptedMediaTypes);
}

public class HyperMediaTypeFormatterProvider : IProvideHyperMediaTypeFormatter
{
    private readonly TypeConfigurationCollection _typeConfigurationCollection;
    private readonly LinkGenerator _linkGenerator;

    public HyperMediaTypeFormatterProvider(HyperMediaConfiguration hyperMediaConfiguration, LinkGenerator linkGenerator)
    {
        _typeConfigurationCollection = hyperMediaConfiguration.ConfiguredTypes;
        _linkGenerator = linkGenerator;
    }

    public IHyperMediaTypeFormatter GetFormatter(IEnumerable<string> acceptedMediaTypes)
    {
        IHyperMediaTypeFormatter? formatter = null;

        foreach (var acceptedMediaType in acceptedMediaTypes)
        {
            if (acceptedMediaType != JsonHalTypeFormatter.MediaType)
                continue;

            formatter = new JsonHalTypeFormatter(_typeConfigurationCollection, _linkGenerator);
            break;
        }

        if (formatter is null)
            throw new NotSupportedException();

        return formatter;
    }
}