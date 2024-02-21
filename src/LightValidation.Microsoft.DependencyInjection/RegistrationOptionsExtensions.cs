using LightValidation.Abstractions;
using LightValidation.Microsoft.DependencyInjection.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace LightValidation.Microsoft.DependencyInjection;

public static class RegistrationOptionsExtensions
{
    public static IRegistrationOptions InjectDependencyResolver(this IRegistrationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Services.TryAddScoped<IDependencyResolver, DependencyResolver>();

        return options.AddModifier(info => info.HasResolverConfiguration, (serviceProvider, validator) =>
        {
            var dependencyResolver = serviceProvider.GetRequiredService<IDependencyResolver>();
            var typed = (IResolverConfiguration)validator;

            typed.SetDependencyResolver(dependencyResolver);
        });
    }
}
