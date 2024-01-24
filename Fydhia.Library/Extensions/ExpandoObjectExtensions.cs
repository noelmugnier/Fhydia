using System.Dynamic;

namespace Fydhia.Library;

public static class ExpandoObjectExtensions
{
    public static ExpandoObject RemoveTypeProperty(this ExpandoObject obj)
    {
        obj.Remove("_type", out _);
        return obj;
    }

    public static ExpandoObject AddTypeProperty(this ExpandoObject obj, Type type)
    {
        obj.TryAdd("_type", type);
        return obj;
    }

    public static Type? GetOriginalType(this ExpandoObject expando)
    {
        return expando.ToDictionary()["_type"] as Type;
    }
}