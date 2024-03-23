using LightValidation.Abstractions;
using LightValidation.Constants;
using System;

namespace LightValidation.Extensions;

public static class ValidationOptionsExtensions
{
    public static IValidationOptions IncludeAllRuleSets(this IValidationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.IncludeRuleSets(RuleSet.Any);

        return options;
    }

    public static IValidationOptions IncludeDefaultRuleSet(this IValidationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.IncludeRuleSets(RuleSet.Default);

        return options;
    }
}
