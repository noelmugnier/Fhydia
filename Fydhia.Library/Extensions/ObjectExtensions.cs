using System.Dynamic;

namespace Fydhia.Library;

public static class ObjectExtensions
{
    public static ExpandoObject ToExpando<T>(this T? obj)
    {
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

    public static IDictionary<string, object> ToDictionary(this object? obj)
    {
        return obj as IDictionary<string, object>;
    }
}