namespace Fydhia.Core.Common;

public record HyperMediaHalLink(string Href, string? Title = null, string? Name = null, bool? Templated = false);