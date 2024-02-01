using Fydhia.Core.Resources;

namespace Fydhia.Core.Common;

public record HyperMediaLink(string Href, string Verb, ReturnedType Type, IEnumerable<ParsedParameter> Parameters, bool Templated = false);