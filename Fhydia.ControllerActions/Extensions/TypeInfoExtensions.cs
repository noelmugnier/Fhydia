using System.ComponentModel;
using System.Reflection;

namespace Fydhia.ControllerActions.Extensions;

public static class TypeInfoExtensions
{
    public static string GetControllerClassName(this TypeInfo controllerType)
    {
        return $"{controllerType.Name.Replace("Controller", string.Empty)}";
    }

    public static string? GetTypeDescription(this TypeInfo typeInfo)
    {
        return typeInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public static string? GetTypeDescription(this MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }
}