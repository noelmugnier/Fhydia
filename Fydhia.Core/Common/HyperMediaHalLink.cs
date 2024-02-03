namespace Fydhia.Core.Common;

public record HyperMediaHalLink(string Href, bool Templated = false, string? Title = null, string? Name = null);