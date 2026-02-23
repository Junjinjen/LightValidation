using LightValidation.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LightValidation;

public interface IValidationContext<TError>
{
    bool HasErrors { get; }

    IReadOnlyDictionary<string, IReadOnlyList<TError>> Errors { get; }

    bool IsPropertyValid(string property);

    IValidationContext<TError> UsePropertyNameProvider(Func<string, string> provider);

    IValidationContext<TError> AddPropertyAlias(string property, string alias);

    IValidationContext<TError> AddPropertyError(string property, TError error);
}

public sealed class ValidationContext<TError> : IValidationContext<TError>
{
    private const char UnderscoreChar = '_';

    private Func<string, string>? _nameProvider;
    private ErrorCollection<TError>? _errors;
    private AliasCollection? _aliases;

    public bool HasErrors => _errors != null;

    public IReadOnlyDictionary<string, IReadOnlyList<TError>> Errors =>
        _errors != null ? _errors : ImmutableDictionary<string, IReadOnlyList<TError>>.Empty;

    public bool IsPropertyValid(string property)
    {
        if (_errors == null)
        {
            return true;
        }

        property = GetProperty(property);

        return _errors.IsPropertyValid(property);
    }

    public IValidationContext<TError> UsePropertyNameProvider(Func<string, string> provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        _nameProvider = provider;

        return this;
    }

    public IValidationContext<TError> AddPropertyAlias(string property, string alias)
    {
        ValidateProperty(property);
        ValidateAlias(alias);

        _aliases ??= new();
        _aliases.AddAlias(property, alias);

        return this;
    }

    public IValidationContext<TError> AddPropertyError(string property, TError error)
    {
        ValidateProperty(property);

        property = GetProperty(property);

        _errors ??= [];
        _errors.Add(property, error, _nameProvider);

        return this;
    }

    private static void ValidateProperty(string property)
    {
        ArgumentNullException.ThrowIfNull(property);

        var i = 0;
        while (i < property.Length)
        {
            var @char = property[i];
            if (IsValidPropertyChar(@char) || @char == Constants.PropertyDelimiter)
            {
                i++;
            }
            else if (@char == Constants.OpeningIndexChar)
            {
                i += GetArgumentsLength(property, i, Constants.OpeningIndexChar, Constants.ClosingIndexChar);
            }
            else if (@char == Constants.OpeningArgumentsChar)
            {
                i += GetArgumentsLength(property, i, Constants.OpeningArgumentsChar, Constants.ClosingArgumentsChar);
            }
            else
            {
                throw new ArgumentException($"Property contains an invalid character: '{@char}'.", nameof(property));
            }
        }
    }

    private static void ValidateAlias(string alias)
    {
        ArgumentNullException.ThrowIfNull(alias);

        for (var i = 0; i < alias.Length; i++)
        {
            var @char = alias[i];
            if (!IsValidPropertyChar(@char))
            {
                throw new ArgumentException($"Alias contains an invalid character: '{@char}'.", nameof(alias));
            }
        }
    }

    private static bool IsValidPropertyChar(char value)
    {
        return char.IsLetterOrDigit(value) || value == UnderscoreChar;
    }

    private static int GetArgumentsLength(string property, int index, char openingChar, char closingChar)
    {
        var depth = 1;
        for (var i = index + 1; i < property.Length; i++)
        {
            var @char = property[i];
            if (@char == closingChar)
            {
                if (--depth == 0)
                {
                    return i - index + 1;
                }
            }
            else if (@char == openingChar)
            {
                depth++;
            }
        }

        throw new ArgumentException($"Property doesn't contain a matching '{closingChar}' character.", nameof(property));
    }

    private string GetProperty(string alias)
    {
        return _aliases != null ? _aliases.GetProperty(alias) : alias;
    }
}
