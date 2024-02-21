using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LightValidation.Abstractions;

public interface IValidationCache : IReadOnlyCollection<KeyValuePair<string, object>>
{
    TValue Get<TValue>(string key);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    TValue? GetOrDefault<TValue>(string key, TValue? defaultValue = default);

    bool TryGet<TValue>(string key, [MaybeNullWhen(false)] out TValue value);

    void Set<TValue>(string key, TValue value);

    void Remove(string key);

    void Clear();
}
