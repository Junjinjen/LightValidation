using System;

namespace LightValidation.SourceGenerator.Models;

internal readonly struct MethodInfo : IEquatable<MethodInfo>
{
    public MethodInfo(
        AttributeInfo attribute, InterfaceInfo @interface, ClassInfo @class, ConstructorInfo[] constructors)
    {
        Attribute = attribute;
        Interface = @interface;
        Class = @class;
        Constructors = constructors;
    }

    public readonly AttributeInfo Attribute;

    public readonly InterfaceInfo Interface;

    public readonly ClassInfo Class;

    public readonly ConstructorInfo[] Constructors;

    public override bool Equals(object? obj)
    {
        return obj is MethodInfo info && Equals(info);
    }

    public bool Equals(MethodInfo other)
    {
        return Attribute.Equals(other.Attribute)
               && Interface.Equals(other.Interface)
               && Class.Equals(other.Class)
               && Constructors.AsSpan().SequenceEqual(other.Constructors.AsSpan());
    }

    public override int GetHashCode()
    {
        var hashCode = -199626678;
        hashCode = (hashCode * -1521134295) + Attribute.GetHashCode();
        hashCode = (hashCode * -1521134295) + Interface.GetHashCode();
        hashCode = (hashCode * -1521134295) + Class.GetHashCode();

        for (var i = 0; i < Constructors.Length; i++)
        {
            hashCode = (hashCode * -1521134295) + Constructors[i].GetHashCode();
        }

        return hashCode;
    }
}
