using System;

namespace LightValidation.SourceGenerator.Models;

internal readonly struct ConstructorInfo : IEquatable<ConstructorInfo>
{
    public ConstructorInfo(string methodParametersSource, string invocationParametersSource)
    {
        MethodParametersSource = methodParametersSource;
        InvocationParametersSource = invocationParametersSource;
    }

    public readonly string MethodParametersSource;

    public readonly string InvocationParametersSource;

    public override bool Equals(object? obj)
    {
        return obj is ConstructorInfo info && Equals(info);
    }

    public bool Equals(ConstructorInfo other)
    {
        return MethodParametersSource == other.MethodParametersSource &&
               InvocationParametersSource == other.InvocationParametersSource;
    }

    public override int GetHashCode()
    {
        var hashCode = 1544803191;
        hashCode = (hashCode * -1521134295) + MethodParametersSource.GetHashCode();
        hashCode = (hashCode * -1521134295) + InvocationParametersSource.GetHashCode();

        return hashCode;
    }
}
