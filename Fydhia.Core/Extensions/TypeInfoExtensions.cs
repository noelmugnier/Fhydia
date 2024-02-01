using System.ComponentModel;
using System.Reflection;

namespace Fydhia.Core.Extensions;

public static class TypeInfoExtensions
{
    public static string? GetTypeDescription(this ParameterInfo parameterInfo)
    {
        return parameterInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public static string? GetTypeDescription(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public static string? GetTypeDescription(this Type type)
    {
        return type.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }
}