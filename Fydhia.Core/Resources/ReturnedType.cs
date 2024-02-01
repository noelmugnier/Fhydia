using System.Reflection;
using Fydhia.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Fydhia.Core.Resources;

public class ReturnedType : Resource
{
    public IEnumerable<ParsedProperty> Properties { get; }

    private ReturnedType(string name, Type type) : base(name, type)
    {
        Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(p => new ParsedProperty(p));
        Description = type.GetTypeDescription();
    }

    public static ReturnedType CreateFrom(MethodInfo methodInfo, TypeInfo controllerType)
    {
        var returnedType = methodInfo.ReturnType;
        if (returnedType.IsGenericType)
        {
            returnedType = RecursivelyFindComplexOrSimpleType(returnedType);
        }

        if (returnedType == typeof(ActionResult) && !returnedType.IsGenericType)
        {
            var produceResponseTypeAttribute = methodInfo.GetCustomAttributes<ProducesResponseTypeAttribute>(true)
                .FirstOrDefault(c => c.Type != null);
            if (produceResponseTypeAttribute != null)
                returnedType = produceResponseTypeAttribute.Type;
        }

        return new ReturnedType(returnedType.Name, returnedType);
    }

    private static Type RecursivelyFindComplexOrSimpleType(Type type)
    {
        if (!type.IsGenericType)
        {
            return type;
        }

        var returnedType = type;
        var genericReturnedType = returnedType.GetGenericTypeDefinition();
        if (returnedType.IsGenericType && (genericReturnedType == typeof(ActionResult<>) ||
                                           genericReturnedType == typeof(Task<>) ||
                                           genericReturnedType == typeof(ValueTask<>)))
        {
            returnedType = RecursivelyFindComplexOrSimpleType(returnedType.GenericTypeArguments[0]);
        }

        return returnedType;
    }
}