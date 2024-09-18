using System;

namespace LightValidation.DependencyInjection;

public sealed class ValidatorInfo
{
    public required Type InterfaceType { get; init; }

    public required Type ValidatorType { get; init; }

    public bool HasInterface<TInterface>()
    {
        return Array.Exists(ValidatorType.GetInterfaces(), x => x == typeof(TInterface));
    }
}
