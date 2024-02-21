using System;

namespace LightValidation.DependencyInjection;

public sealed class ValidatorInfo
{
    public required Type InterfaceType { get; init; }

    public required Type ValidatorType { get; init; }

    public required bool HasResolverConfiguration { get; init; }
}
