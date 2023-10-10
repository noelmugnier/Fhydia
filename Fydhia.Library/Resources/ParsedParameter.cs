using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fydhia.Library;

public class ParsedParameter : Resource
{
    public readonly ParameterInfo ParameterInfo;

    public ParsedParameter(ParameterInfo parameterInfo) : base(parameterInfo.Name, parameterInfo.ParameterType)
    {
        ParameterInfo = parameterInfo;
        BindingSource = ParseParameterAttribute(parameterInfo);
        Description = parameterInfo.GetTypeDescription();
    }

    private BindingSource? ParseParameterAttribute(ParameterInfo parameterInfo)
    {
        var availableAttributeTypes = new[]
        {
            typeof(FromQueryAttribute), typeof(FromRouteAttribute), typeof(FromBodyAttribute),
            typeof(FromHeaderAttribute), typeof(FromFormAttribute)
        };

        foreach (var attributeType in availableAttributeTypes)
        {
            var bindingSource = ExtractBindingSourceAttribute(parameterInfo, attributeType);
            if (bindingSource != null)
            {
                return bindingSource;
            }
        }

        return null;
    }

    private static BindingSource ExtractBindingSourceAttribute(ParameterInfo parameterInfo,
        Type bindingSourceMetadataAttribute)
    {
        var attribute = parameterInfo.GetCustomAttribute(bindingSourceMetadataAttribute);
        return attribute is not null and IBindingSourceMetadata bindingSourceMetadata
            ? bindingSourceMetadata.BindingSource
            : null;
    }

    public BindingSource? BindingSource { get; }
}