using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Formatters;

public interface IProvideHyperMediaTypesFormatter
{
    IHyperMediaTypesFormatter GetFormatter(IEnumerable<string> acceptedMediaTypes);
}

public class HyperMediaTypesFormatterProvider : IProvideHyperMediaTypesFormatter
{
    private readonly TypeConfigurationCollection _typeConfigurationCollection;
    private readonly LinkGenerator _linkGenerator;

    public HyperMediaTypesFormatterProvider(HyperMediaConfiguration hyperMediaConfiguration, LinkGenerator linkGenerator)
    {
        _typeConfigurationCollection = hyperMediaConfiguration.ConfiguredTypes;
        _linkGenerator = linkGenerator;
    }

    public IHyperMediaTypesFormatter GetFormatter(IEnumerable<string> acceptedMediaTypes)
    {
        IHyperMediaTypesFormatter? formatter = null;

        foreach (var acceptedMediaType in acceptedMediaTypes)
        {
            if (acceptedMediaType != JsonHalTypesFormatter.MediaType)
                continue;

            formatter = new JsonHalTypesFormatter(_typeConfigurationCollection, _linkGenerator);
            break;
        }

        if (formatter is null)
            throw new NotSupportedException();

        return formatter;
    }
}