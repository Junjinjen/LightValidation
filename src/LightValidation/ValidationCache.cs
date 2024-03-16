using LightValidation.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LightValidation;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<KeyValuePair<string, object>>))]
public sealed class ValidationCache : IReadOnlyCollection<KeyValuePair<string, object>>
{
    private Dictionary<string, object?>? _cache;

    public int Count => _cache?.Count ?? 0;

    public TValue Get<TValue>(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        if (_cache == null)
        {
            throw new KeyNotFoundException();
        }

        return (TValue)_cache[key]!;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TValue? GetOrDefault<TValue>(string key, TValue? defaultValue = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        return TryGet<TValue>(key, out var value) ? value : defaultValue;
    }

    public bool TryGet<TValue>(string key, [MaybeNullWhen(false)] out TValue value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        if (_cache?.TryGetValue(key, out var stored) == true && stored is TValue typed)
        {
            value = typed;

            return true;
        }

        value = default;

        return false;
    }

    public void Set<TValue>(string key, TValue value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        _cache ??= [];
        _cache[key] = value;
    }

    public void Remove(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        _cache?.Remove(key);
    }

    public void Clear()
    {
        _cache?.Clear();
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        if (_cache != null)
        {
            return _cache.GetEnumerator();
        }

        return Enumerable.Empty<KeyValuePair<string, object>>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
