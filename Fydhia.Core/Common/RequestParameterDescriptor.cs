using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fydhia.Core.Common;

public record RequestParameterDescriptor(ParameterInfo ParameterInfo)
{
    public string? BinderModelName { get; init; }
    public BindingSource? BindingSource { get; init; }
    public string Name => ParameterInfo.Name ?? string.Empty;
}