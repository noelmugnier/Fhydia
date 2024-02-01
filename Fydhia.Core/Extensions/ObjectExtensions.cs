using System.Dynamic;

namespace Fydhia.Core.Extensions;

public static class ObjectExtensions
{
    public static ExpandoObject ToExpando<T>(this T? obj)
    {
        if (obj is null)
            return new ExpandoObject();

        var expando = new ExpandoObject();

        var type = obj.GetType();
        expando.AddTypeProperty(type);

        foreach (var propertyInfo in type.GetProperties())
        {
            var currentValue = propertyInfo.GetValue(obj);
            expando.TryAdd(propertyInfo.Name, currentValue);
        }

        return expando;
    }

    public static IDictionary<string, object?> ToDictionary(this object? obj)
    {
        return obj as IDictionary<string, object?> ?? new Dictionary<string, object?>();
    }

    public static bool IsDefault<T>(this T obj)
    {
        return obj?.ToString() == obj?.GetType().GetDefault()?.ToString();
    }
}