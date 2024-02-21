using LightValidation.DependencyInjection;
using LightValidation.Microsoft.DependencyInjection.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LightValidation.Microsoft.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidatorsFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        Action<IRegistrationOptions>? optionsAction = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var infos = AssemblyScanner.FindValidators(assembly);
        var options = new RegistrationOptions(infos)
        {
            Services = services,
        };

        optionsAction ??= options => options.InjectDependencyResolver();
        optionsAction.Invoke(options);

        options.Register(lifetime);

        return services;
    }

    public static IServiceCollection AddValidatorsFromAssembly(
        this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddValidatorsFromAssembly(assembly, optionsAction: null, lifetime);
    }

    public static IServiceCollection AddValidatorsFromCurrentAssembly(
        this IServiceCollection services,
        Action<IRegistrationOptions>? optionsAction = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddValidatorsFromAssembly(Assembly.GetCallingAssembly(), optionsAction, lifetime);
    }

    public static IServiceCollection AddValidatorsFromCurrentAssembly(
        this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddValidatorsFromAssembly(Assembly.GetCallingAssembly(), optionsAction: null, lifetime);
    }

    public static IServiceCollection AddValidatorsFromAssemblies(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IRegistrationOptions>? optionsAction = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        var infos = AssemblyScanner.FindValidators(assemblies);
        var options = new RegistrationOptions(infos)
        {
            Services = services,
        };

        optionsAction ??= options => options.InjectDependencyResolver();
        optionsAction.Invoke(options);

        options.Register(lifetime);

        return services;
    }

    public static IServiceCollection AddValidatorsFromAssemblies(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddValidatorsFromAssemblies(assemblies, optionsAction: null, lifetime);
    }
}
