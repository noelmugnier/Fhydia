namespace Fydhia.Library;

public record HyperMediaLink(string Href, string Verb, ReturnedType Type, IEnumerable<ParsedParameter> Parameters, bool Templated = false);