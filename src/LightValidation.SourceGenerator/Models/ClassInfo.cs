using System;

namespace LightValidation.SourceGenerator.Models;

internal readonly struct ClassInfo : IEquatable<ClassInfo>
{
    public ClassInfo(string genericParametersSource, string constraintsSource, string fullName)
    {
        GenericParametersSource = genericParametersSource;
        ConstraintsSource = constraintsSource;
        FullName = fullName;
    }

    public readonly string GenericParametersSource;

    public readonly string ConstraintsSource;

    public readonly string FullName;

    public override bool Equals(object? obj)
    {
        return obj is ClassInfo info && Equals(info);
    }

    public bool Equals(ClassInfo other)
    {
        return GenericParametersSource == other.GenericParametersSource &&
               ConstraintsSource == other.ConstraintsSource &&
               FullName == other.FullName;
    }

    public override int GetHashCode()
    {
        var hashCode = -1391673460;
        hashCode = (hashCode * -1521134295) + GenericParametersSource.GetHashCode();
        hashCode = (hashCode * -1521134295) + ConstraintsSource.GetHashCode();
        hashCode = (hashCode * -1521134295) + FullName.GetHashCode();

        return hashCode;
    }
}
