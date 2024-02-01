using System.Reflection;
using Fydhia.ControllerActions.Extensions;
using Fydhia.Core.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Fhydia.ControllerActions.Resources;

public class ControllerGroup : Resource
{
    private ControllerGroup(MethodInfo methodInfo, TypeInfo controllerTypeInfo) : base(
        GetControllerGroupName(methodInfo, controllerTypeInfo), controllerTypeInfo)
    {
    }

    public static ControllerGroup CreateFrom(MethodInfo methodInfo, TypeInfo controllerTypeInfo)
    {
        return new ControllerGroup(methodInfo, controllerTypeInfo)
        {
            Description = controllerTypeInfo.GetTypeDescription()
        };
    }

    private static string GetControllerGroupName(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var controllerName = controllerType.GetControllerClassName();
        var methodApiExplorerSettingAttributeName = methodInfo.GetCustomAttributes<ApiExplorerSettingsAttribute>(true)
            .FirstOrDefault()?.GroupName?.Trim();
        var controllerApiExplorerSettingAttributeName = controllerType
            .GetCustomAttributes<ApiExplorerSettingsAttribute>(true).FirstOrDefault()?.GroupName?.Trim();

        if (!string.IsNullOrWhiteSpace(methodApiExplorerSettingAttributeName))
        {
            return methodApiExplorerSettingAttributeName;
        }

        if (!string.IsNullOrWhiteSpace(controllerApiExplorerSettingAttributeName))
        {
            return controllerApiExplorerSettingAttributeName;
        }

        return controllerName;
    }
}