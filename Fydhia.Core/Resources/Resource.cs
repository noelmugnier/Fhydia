using System.Reflection;

namespace Fydhia.Core.Resources;

public abstract class Resource
{
    protected Resource(string name, Type relatedType) : this(name, relatedType.GetTypeInfo())
    {
    }

    protected Resource(string name, TypeInfo relatedType)
    {
        Name = name;
        TypeInfo = relatedType;
    }

    public string? Description { get; protected set; }
    public string Name { get; }
    public TypeInfo TypeInfo { get; }
}