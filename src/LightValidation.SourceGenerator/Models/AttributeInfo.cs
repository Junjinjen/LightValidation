using System;

namespace LightValidation.SourceGenerator.Models;

internal readonly struct AttributeInfo : IEquatable<AttributeInfo>
{
    public AttributeInfo(string accessModifier, bool savePropertyNullability, string methodName, string @namespace)
    {
        AccessModifier = accessModifier;
        SavePropertyNullability = savePropertyNullability;
        MethodName = methodName;
        Namespace = @namespace;
    }

    public readonly string AccessModifier;

    public readonly bool SavePropertyNullability;

    public readonly string MethodName;

    public readonly string Namespace;

    public override bool Equals(object? obj)
    {
        return obj is AttributeInfo info && Equals(info);
    }

    public bool Equals(AttributeInfo other)
    {
        return AccessModifier == other.AccessModifier
            && SavePropertyNullability == other.SavePropertyNullability
            && MethodName == other.MethodName
            && Namespace == other.Namespace;
    }

    public override int GetHashCode()
    {
        var hashCode = -1076683149;
        hashCode = (hashCode * -1521134295) + AccessModifier.GetHashCode();
        hashCode = (hashCode * -1521134295) + SavePropertyNullability.GetHashCode();
        hashCode = (hashCode * -1521134295) + MethodName.GetHashCode();
        hashCode = (hashCode * -1521134295) + Namespace.GetHashCode();

        return hashCode;
    }
}
