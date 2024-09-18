using LightValidation.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LightValidation.Microsoft.DependencyInjection.Internal;

internal sealed class ValidatorDescriptor
{
    private List<Action<IServiceProvider, object>>? _modifiers;

    public ValidatorDescriptor(ValidatorInfo validator)
    {
        Validator = validator;
    }

    public ValidatorInfo Validator { get; }

    public void AddModifier(Action<IServiceProvider, object> modifier)
    {
        ArgumentNullException.ThrowIfNull(modifier);

        _modifiers ??= [];
        _modifiers.Add(modifier);
    }

    public ServiceDescriptor GetServiceDescriptor(ModifiersCache cache, ServiceLifetime lifetime)
    {
        if (_modifiers == null)
        {
            return new ServiceDescriptor(Validator.InterfaceType, Validator.ValidatorType, lifetime);
        }

        var modifier = cache.GetCombinedModifier(_modifiers);
        var validatorFactory = ActivatorUtilities.CreateFactory(Validator.ValidatorType, Type.EmptyTypes);

        return new ServiceDescriptor(Validator.InterfaceType, serviceProvider =>
        {
            var validator = validatorFactory.Invoke(serviceProvider, null);
            modifier.Invoke(serviceProvider, validator);

            return validator;
        }, lifetime);
    }
}
