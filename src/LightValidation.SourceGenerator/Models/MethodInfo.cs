using System;

namespace LightValidation.SourceGenerator.Models;

internal readonly struct MethodInfo : IEquatable<MethodInfo>
{
    public MethodInfo(
        AttributeInfo attributeInfo,
        InterfaceInfo interfaceInfo,
        ClassInfo classInfo,
        ConstructorInfo[] constructorInfos)
    {
        AttributeInfo = attributeInfo;
        InterfaceInfo = interfaceInfo;
        ClassInfo = classInfo;
        ConstructorInfos = constructorInfos;
    }

    public readonly AttributeInfo AttributeInfo;

    public readonly InterfaceInfo InterfaceInfo;

    public readonly ClassInfo ClassInfo;

    public readonly ConstructorInfo[] ConstructorInfos;

    public override bool Equals(object? obj)
    {
        return obj is MethodInfo info && Equals(info);
    }

    public bool Equals(MethodInfo other)
    {
        return AttributeInfo.Equals(other.AttributeInfo) &&
               InterfaceInfo.Equals(other.InterfaceInfo) &&
               ClassInfo.Equals(other.ClassInfo) &&
               ConstructorInfos.AsSpan().SequenceEqual(other.ConstructorInfos.AsSpan());
    }

    public override int GetHashCode()
    {
        var hashCode = -199626678;
        hashCode = (hashCode * -1521134295) + AttributeInfo.GetHashCode();
        hashCode = (hashCode * -1521134295) + InterfaceInfo.GetHashCode();
        hashCode = (hashCode * -1521134295) + ClassInfo.GetHashCode();

        for (var i = 0; i < ConstructorInfos.Length; i++)
        {
            hashCode = (hashCode * -1521134295) + ConstructorInfos[i].GetHashCode();
        }

        return hashCode;
    }
}
