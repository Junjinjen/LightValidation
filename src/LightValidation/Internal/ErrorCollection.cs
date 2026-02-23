using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LightValidation.Internal;

internal sealed class ErrorCollection<TError> : IReadOnlyDictionary<string, IReadOnlyList<TError>>
{
    private readonly List<Property> _properties = new(1);

    public IEnumerable<string> Keys => _properties.Select(x => x.Name);

    public IEnumerable<IReadOnlyList<TError>> Values => _properties.Select(x => x.Errors);

    public int Count => _properties.Count;

    public IReadOnlyList<TError> this[string key]
    {
        get
        {
            var index = FindNameIndex(key);
            if (index < 0)
            {
                throw new KeyNotFoundException();
            }

            return _properties[index].Errors;
        }
    }

    public bool ContainsKey(string key)
    {
        var index = FindNameIndex(key);

        return index >= 0;
    }

    public IEnumerator<KeyValuePair<string, IReadOnlyList<TError>>> GetEnumerator()
    {
        foreach (var property in _properties)
        {
            yield return KeyValuePair.Create(property.Name, (IReadOnlyList<TError>)property.Errors);
        }
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out IReadOnlyList<TError> value)
    {
        var index = FindNameIndex(key);
        var result = index >= 0;
        value = result ? _properties[index].Errors : default;

        return result;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(string property, TError error, Func<string, string>? nameProvider)
    {
        var index = FindPropertyIndex(property);
        if (index >= 0)
        {
            _properties[index].Errors.Add(error);
        }
        else
        {
            var name = nameProvider?.Invoke(property) ?? property;
            _properties.Add(new Property(name, property, error));
        }
    }

    public bool IsPropertyValid(string property)
    {
        for (var i = 0; i < _properties.Count; i++)
        {
            if (PropertyNameUtilities.StartsWith(_properties[i].Path, property))
            {
                return false;
            }
        }

        return true;
    }

    private int FindNameIndex(string name)
    {
        for (var i = 0; i < _properties.Count; i++)
        {
            if (PropertyNameUtilities.Equals(_properties[i].Name, name))
            {
                return i;
            }
        }

        return -1;
    }

    private int FindPropertyIndex(string property)
    {
        for (var i = 0; i < _properties.Count; i++)
        {
            if (PropertyNameUtilities.Equals(_properties[i].Path, property))
            {
                return i;
            }
        }

        return -1;
    }

    private readonly struct Property
    {
        public Property(string name, string path, TError error)
        {
            Name = name;
            Path = path;
            Errors = [error];
        }

        public readonly string Name;

        public readonly string Path;

        public readonly List<TError> Errors;
    }
}
