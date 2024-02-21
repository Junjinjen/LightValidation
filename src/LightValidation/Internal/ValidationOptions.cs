using LightValidation.Abstractions;
using System;
using System.Collections.Generic;

namespace LightValidation.Internal;

internal interface IValidationOptionsInternal : IValidationOptions
{
    RuleSetCollection? RuleSets { get; }

    IValidationCache? ValidationCache { get; }
}

internal sealed class ValidationOptions : IValidationOptionsInternal
{
    private List<string>? _ruleSets;

    public RuleSetCollection? RuleSets => _ruleSets?.Count > 0 ? new RuleSetCollection(_ruleSets) : null;

    public IValidationCache? ValidationCache { get; private set; }

    public IValidationOptions IncludeRuleSets(params string[] ruleSets)
    {
        ArgumentNullException.ThrowIfNull(ruleSets);
        if (Array.Exists(ruleSets, string.IsNullOrEmpty))
        {
            throw new ArgumentException(
                "The rule set collection cannot contain null or empty rule sets.", nameof(ruleSets));
        }

        _ruleSets ??= new List<string>(ruleSets.Length);
        _ruleSets.AddRange(ruleSets);

        return this;
    }

    public IValidationOptions UseValidationCache(IValidationCache cache)
    {
        ArgumentNullException.ThrowIfNull(cache);

        ValidationCache = cache;

        return this;
    }
}
