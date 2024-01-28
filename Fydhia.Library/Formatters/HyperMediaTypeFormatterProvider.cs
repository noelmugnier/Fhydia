using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public interface IProvideHyperMediaTypeFormatter
{
    HypermediaTypeFormatter GetFormatter(IEnumerable<string> acceptedMediaTypes);
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

    public HypermediaTypeFormatter GetFormatter(IEnumerable<string> acceptedMediaTypes)
    {
        HypermediaTypeFormatter? formatter = null;

        foreach (var acceptedMediaType in acceptedMediaTypes)
        {
            if (acceptedMediaType != "application/hal+json")
                continue;

            formatter = new JsonHalTypeFormatter(_typeConfigurationCollection, _linkGenerator);
            break;
        }

        if (formatter is null)
            throw new NotSupportedException();

        return formatter;
    }
}