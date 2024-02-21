using System;
using System.Collections.Generic;
using System.Linq;

namespace LightValidation.Microsoft.DependencyInjection.Internal;

internal sealed class ModifiersCache
{
    private readonly List<KeyValuePair<List<Action<IServiceProvider, object>>, Action<IServiceProvider, object>>> _cache = [];

    public Action<IServiceProvider, object> GetCombinedModifier(List<Action<IServiceProvider, object>> modifiers)
    {
        if (modifiers.Count == 1)
        {
            return modifiers.Single();
        }

        var index = _cache.FindIndex(x => x.Key.SequenceEqual(modifiers));
        if (index >= 0)
        {
            return _cache[index].Value;
        }

        var array = modifiers.ToArray();
        Action<IServiceProvider, object> modifier = (serviceProvider, validator) =>
        {
            foreach (var modifier in array)
            {
                modifier.Invoke(serviceProvider, validator);
            }
        };

        var pair = KeyValuePair.Create(modifiers, modifier);
        _cache.Add(pair);

        return modifier;
    }
}
