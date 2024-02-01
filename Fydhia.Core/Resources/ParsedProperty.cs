using System.Reflection;
using Fydhia.Core.Extensions;

namespace Fydhia.Core.Resources;

public class ParsedProperty : Resource
{
    public readonly PropertyInfo PropertyInfo;

    public ParsedProperty(PropertyInfo propertyInfo) : base(propertyInfo.Name, propertyInfo.PropertyType)
    {
        PropertyInfo = propertyInfo;
        Description = propertyInfo.GetTypeDescription();
    }
}