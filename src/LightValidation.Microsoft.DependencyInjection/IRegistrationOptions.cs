using LightValidation.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LightValidation.Microsoft.DependencyInjection;

public interface IRegistrationOptions
{
    IServiceCollection Services { get; }

    IRegistrationOptions AddModifier(Func<ValidatorInfo, bool> filter, Action<IServiceProvider, object> modifier);

    IRegistrationOptions AddModifier(Action<IServiceProvider, object> modifier);
}
