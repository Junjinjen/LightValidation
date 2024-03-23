using System;

namespace LightValidation.SourceGenerator.Models;

internal readonly struct ConstructorInfo : IEquatable<ConstructorInfo>
{
    public ConstructorInfo(string parametersSource, string argumentsSource)
    {
        ParametersSource = parametersSource;
        ArgumentsSource = argumentsSource;
    }

    public readonly string ParametersSource;

    public readonly string ArgumentsSource;

    public override bool Equals(object? obj)
    {
        return obj is ConstructorInfo info && Equals(info);
    }

    public bool Equals(ConstructorInfo other)
    {
        return ParametersSource == other.ParametersSource &&
               ArgumentsSource == other.ArgumentsSource;
    }

    public override int GetHashCode()
    {
        var hashCode = 1544803191;
        hashCode = (hashCode * -1521134295) + ParametersSource.GetHashCode();
        hashCode = (hashCode * -1521134295) + ArgumentsSource.GetHashCode();

        return hashCode;
    }
}
