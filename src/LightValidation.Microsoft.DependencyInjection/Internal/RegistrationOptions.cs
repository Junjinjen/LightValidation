using LightValidation.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightValidation.Microsoft.DependencyInjection.Internal;

internal sealed class RegistrationOptions : IRegistrationOptions
{
    private readonly ValidatorDescriptor[] _validatorDescriptors;

    public RegistrationOptions(IEnumerable<ValidatorInfo> validators)
    {
        _validatorDescriptors = validators.Select(x => new ValidatorDescriptor(x)).ToArray();
    }

    public required IServiceCollection Services { get; init; }

    public IRegistrationOptions AddModifier(
        Func<ValidatorInfo, bool> filter, Action<IServiceProvider, object> modifier)
    {
        foreach (var descriptor in _validatorDescriptors.Where(x => filter.Invoke(x.Validator)))
        {
            descriptor.AddModifier(modifier);
        }

        return this;
    }

    public IRegistrationOptions AddModifier(Action<IServiceProvider, object> modifier)
    {
        foreach (var descriptor in _validatorDescriptors)
        {
            descriptor.AddModifier(modifier);
        }

        return this;
    }

    public void Register(ServiceLifetime lifetime)
    {
        var cache = new ModifiersCache();

        var descriptors = _validatorDescriptors.Select(x => x.GetServiceDescriptor(cache, lifetime));
        foreach (var descriptor in descriptors)
        {
            Services.Add(descriptor);
        }
    }
}
