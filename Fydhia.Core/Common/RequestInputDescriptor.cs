using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fydhia.Core.Common;

public record RequestInputDescriptor(ParameterInfo ParameterInfo)
{
    public string BinderModelName { get; init; } = default!;
    public BindingSource? BindingSource { get; init; }
    public string Name => ParameterInfo.Name ?? string.Empty;
}