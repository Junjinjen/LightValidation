using System;

namespace LightValidation.SourceGenerator.Models;

internal readonly struct InterfaceInfo : IEquatable<InterfaceInfo>
{
    public InterfaceInfo(
        bool hasNullablePropertyTypeModifier,
        string ruleChainMethod,
        string entityTypeSource,
        string propertyTypeSource)
    {
        HasNullablePropertyTypeModifier = hasNullablePropertyTypeModifier;
        RuleChainMethod = ruleChainMethod;
        EntityTypeSource = entityTypeSource;
        PropertyTypeSource = propertyTypeSource;
    }

    public readonly bool HasNullablePropertyTypeModifier;

    public readonly string RuleChainMethod;

    public readonly string EntityTypeSource;

    public readonly string PropertyTypeSource;

    public override bool Equals(object? obj)
    {
        return obj is InterfaceInfo info && Equals(info);
    }

    public bool Equals(InterfaceInfo other)
    {
        return HasNullablePropertyTypeModifier == other.HasNullablePropertyTypeModifier
               && RuleChainMethod == other.RuleChainMethod
               && EntityTypeSource == other.EntityTypeSource
               && PropertyTypeSource == other.PropertyTypeSource;
    }

    public override int GetHashCode()
    {
        var hashCode = 1504215869;
        hashCode = (hashCode * -1521134295) + HasNullablePropertyTypeModifier.GetHashCode();
        hashCode = (hashCode * -1521134295) + RuleChainMethod.GetHashCode();
        hashCode = (hashCode * -1521134295) + EntityTypeSource.GetHashCode();
        hashCode = (hashCode * -1521134295) + PropertyTypeSource.GetHashCode();

        return hashCode;
    }
}
