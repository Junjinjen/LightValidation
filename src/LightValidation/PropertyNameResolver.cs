using System;
using System.Reflection;
using System.Text;

namespace LightValidation;

public interface IPropertyNameResolver
{
    string? GetModelName(Type modelType);

    string? GetMemberName(MemberInfo member);

    string? GetMethodName(MethodInfo method);

    CompositeFormat? GetArgumentsFormat();

    CompositeFormat? GetIndexFormat();
}

public class PropertyNameResolver : IPropertyNameResolver
{
    private static readonly CompositeFormat ArgumentsFormat = CompositeFormat.Parse("({0})");
    private static readonly CompositeFormat IndexFormat = CompositeFormat.Parse("[{0}]");

    public virtual string? GetModelName(Type modelType)
    {
        return string.Empty;
    }

    public virtual string? GetMemberName(MemberInfo member)
    {
        return member.Name;
    }

    public virtual string? GetMethodName(MethodInfo method)
    {
        return method.Name;
    }

    public virtual CompositeFormat? GetArgumentsFormat()
    {
        return ArgumentsFormat;
    }

    public virtual CompositeFormat? GetIndexFormat()
    {
        return IndexFormat;
    }
}
