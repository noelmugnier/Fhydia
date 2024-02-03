using System.Reflection;

namespace Fhydia.Controllers.Extensions;

public static class TypeInfoExtensions
{
    public static string GetControllerClassName(this TypeInfo controllerType)
    {
        return $"{controllerType.Name.Replace("Controller", string.Empty)}";
    }
}