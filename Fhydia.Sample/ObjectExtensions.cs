using System.Dynamic;

namespace Fhydia.Sample;

public static class ObjectExtensions
{
    public static ExpandoObject ToExpando<T>(this T? obj)
    {
        var expando = new ExpandoObject();

        foreach (var propertyInfo in obj.GetType().GetProperties())
        {
            var currentValue = propertyInfo.GetValue(obj);
            expando.TryAdd(propertyInfo.Name, currentValue);
        }

        return expando;
    }

    public static IDictionary<string, object> ToDictionary(this object? obj)
    {
        var dict = new Dictionary<string, object>();

        foreach (var propertyInfo in obj?.GetType().GetProperties())
        {
            var currentValue = propertyInfo.GetValue(obj);
            dict.TryAdd(propertyInfo.Name, currentValue);
        }

        return dict;
    }
}
