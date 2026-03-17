using System.Collections.Generic;

namespace LightValidation.Internal;

internal sealed class AliasCollection
{
    private readonly List<KeyValuePair<string, string>> _aliases = new(1);

    public string GetProperty(string alias)
    {
        return GetPropertyWithAliasSearch(alias, aliasToSearch: null, out _);
    }

    public void AddAlias(string property, string alias)
    {
        property = GetPropertyWithAliasSearch(property, alias, out var index);
        var pair = KeyValuePair.Create(alias, property);
        if (index >= 0)
        {
            _aliases[index] = pair;
        }
        else
        {
            _aliases.Add(pair);
        }
    }

    private string GetPropertyWithAliasSearch(string alias, string? aliasToSearch, out int index)
    {
        string? property = null;
        var maxLength = -1;
        index = -1;

        for (var i = 0; i < _aliases.Count; i++)
        {
            var currentAlias = _aliases[i].Key;
            var currentLength = currentAlias.Length;
            if (PropertyNameUtilities.StartsWith(alias, currentAlias) && currentLength > maxLength)
            {
                maxLength = currentLength;
                property = _aliases[i].Value;
            }

            if (aliasToSearch != null && PropertyNameUtilities.Equals(aliasToSearch, currentAlias))
            {
                index = i;
            }
        }

        if (maxLength < 0)
        {
            return alias;
        }

        if (maxLength == alias.Length)
        {
            return property!;
        }

        return $"{property}{alias[maxLength..]}";
    }
}
