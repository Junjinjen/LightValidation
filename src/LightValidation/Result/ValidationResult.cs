using System.Collections.Generic;

namespace LightValidation.Result;

public class ValidationResult
{
    public bool IsValid => BrokenRules.Count == 0;

    public required ValidationCache Cache { get; init; }

    public required RuleSetCollection ExecutedRuleSets { get; init; }

    public required IReadOnlyList<RuleFailure> BrokenRules { get; init; }

    public override string ToString()
    {
        return IsValid ? "Valid result" : $"Invalid result [Broken rule count: {BrokenRules.Count}]";
    }
}
