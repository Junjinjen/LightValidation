using System;

namespace LightValidation.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ValidatorInterfaceAttribute : Attribute
{
    public ValidatorInterfaceAttribute(Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);

        InterfaceType = interfaceType;
    }

    public Type InterfaceType { get; }
}
