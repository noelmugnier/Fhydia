using Microsoft.AspNetCore.Routing;

namespace Fydhia.Library;

public class HyperMediaEnricherBuilder
{
    private readonly LinkGenerator _linkGenerator;
    public readonly List<TypeEnricherBuilder> _typeEnricherBuilders = new();

    public HyperMediaEnricherBuilder(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public TypeEnricherBuilder<T> ConfigureEnricherForType<T>() where T : class, new()
    {
        var typeEnricherBuilder = new TypeEnricherBuilder<T>(this, _linkGenerator);
        _typeEnricherBuilders.Add(typeEnricherBuilder);
        return typeEnricherBuilder;
    }

    public HyperMediaEnricher Build()
    {
        var typeEnrichers = _typeEnricherBuilders.Select(builder => builder.Build());
        return new HyperMediaEnricher(typeEnrichers);
    }
}