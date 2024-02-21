using LightValidation.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LightValidation.Microsoft.DependencyInjection.Internal;

internal sealed class ValidatorDescriptor
{
    private List<Action<IServiceProvider, object>>? _modifiers;

    public ValidatorDescriptor(ValidatorInfo validatorInfo)
    {
        ValidatorInfo = validatorInfo;
    }

    public ValidatorInfo ValidatorInfo { get; }

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
            return new ServiceDescriptor(ValidatorInfo.InterfaceType, ValidatorInfo.ValidatorType, lifetime);
        }

        var modifier = cache.GetCombinedModifier(_modifiers);
        var validatorFactory = ActivatorUtilities.CreateFactory(ValidatorInfo.ValidatorType, Type.EmptyTypes);

        return new ServiceDescriptor(ValidatorInfo.InterfaceType, serviceProvider =>
        {
            var validator = validatorFactory.Invoke(serviceProvider, null);
            modifier.Invoke(serviceProvider, validator);

            return validator;
        }, lifetime);
    }
}
