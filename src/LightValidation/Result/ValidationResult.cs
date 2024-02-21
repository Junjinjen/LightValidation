using LightValidation.Abstractions;
using System.Collections.Generic;

namespace LightValidation.Result;

public class ValidationResult
{
    public bool IsValid => BrokenRules.Count == 0;

    public required RuleSetCollection ExecutedRuleSets { get; init; }

    public required IReadOnlyList<RuleFailure> BrokenRules { get; init; }

    public required IValidationCache ValidationCache { get; init; }

    public override string ToString()
    {
        return IsValid ? "Valid result" : $"Invalid result [Broken rules count: {BrokenRules.Count}]";
    }
}
