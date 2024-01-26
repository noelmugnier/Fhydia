using System.Dynamic;

namespace Fydhia.Library;

public abstract class HypermediaTypeFormatter
{
    public abstract ExpandoObject Format(ExpandoObject responseObject, TypeEnricherConfiguration typeEnricherConfiguration);
}

public static class HypermediaTypeFormatterFactory
{
    public static HypermediaTypeFormatter Create(IEnumerable<string> acceptedMediaTypes, LinkFormatter linkGenerator)
    {
        HypermediaTypeFormatter? formatter = null;

        foreach (var acceptedMediaType in acceptedMediaTypes)
        {
            if (acceptedMediaType != "application/hal+json")
                continue;

            formatter = new JsonHalTypeFormatter(linkGenerator);
            break;
        }

        if (formatter is null)
            throw new NotSupportedException();

        return formatter;
    }
}