using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public abstract class HypermediaTypeFormatter
{
    public abstract ExpandoObject Format(ExpandoObject responseObject, TypeEnricherConfiguration typeEnricherConfiguration, HttpContext httpContext);
}

public interface IProvideHyperMediaTypeFormatter
{
    HypermediaTypeFormatter GetFormatter(IEnumerable<string> acceptedMediaTypes);
}

public class HyperMediaTypeFormatterProvider : IProvideHyperMediaTypeFormatter
{
    private readonly LinkGenerator _linkGenerator;

    public HyperMediaTypeFormatterProvider(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public HypermediaTypeFormatter GetFormatter(IEnumerable<string> acceptedMediaTypes)
    {
        HypermediaTypeFormatter? formatter = null;

        foreach (var acceptedMediaType in acceptedMediaTypes)
        {
            if (acceptedMediaType != "application/hal+json")
                continue;

            formatter = new JsonHalTypeFormatter(_linkGenerator);
            break;
        }

        if (formatter is null)
            throw new NotSupportedException();

        return formatter;
    }
}