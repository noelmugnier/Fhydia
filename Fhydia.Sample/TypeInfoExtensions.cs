using System.ComponentModel;
using System.Reflection;

namespace Fhydia.Sample;

public static class TypeInfoExtensions
{
    public static string GetControllerClassName(this TypeInfo controllerType)
    {
        return $"{controllerType.Name.Replace("Controller", string.Empty)}";
    }

    public static string? GetTypeDescription(this Type type)
    {
        return type.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public static string? GetTypeDescription(this TypeInfo typeInfo)
    {
        return typeInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public static string? GetTypeDescription(this MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public static string? GetTypeDescription(this ParameterInfo parameterInfo)
    {
        return parameterInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public static string? GetTypeDescription(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }
}

