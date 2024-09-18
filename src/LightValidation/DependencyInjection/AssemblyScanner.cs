using LightValidation.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightValidation.DependencyInjection;

public static class AssemblyScanner
{
    public static ValidatorInfo[] FindValidators(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return FindValidatorsInternal(assembly).ToArray();
    }

    public static ValidatorInfo[] FindValidators(IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        return assemblies.SelectMany(FindValidatorsInternal).ToArray();
    }

    private static IEnumerable<ValidatorInfo> FindValidatorsInternal(Assembly assembly)
    {
        var query =
            from type in assembly.GetTypes()
            where !type.IsAbstract && !type.IsGenericTypeDefinition
            from interfaceType in type.GetInterfaces()
            where interfaceType.IsGenericType
            where interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>)
            let customInterfaceType = type.GetCustomAttribute<ValidatorInterfaceAttribute>()?.InterfaceType
            select new ValidatorInfo
            {
                InterfaceType = customInterfaceType ?? interfaceType,
                ValidatorType = type,
            };

        return query;
    }
}
