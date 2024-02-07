using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fydhia.Core.Common;

public record RequestParameterDescriptor
{
    public ParameterInfo ParameterInfo { get; init; }
    public string? BinderModelName { get; init; }
    public BindingSource? BindingSource { get; init; }
    public string Name => ParameterInfo.Name ?? string.Empty;
}