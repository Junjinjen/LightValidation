using System;
using System.Collections.Generic;

namespace LightValidation.Result;

public class ValidationException : Exception
{
    private static readonly RuleSetCollection EmptyRuleSetCollection = new([]);

    public ValidationException()
        : this(message: null, innerException: null)
    {
    }

    public ValidationException(string? message)
        : this(message, innerException: null)
    {
    }

    public ValidationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        Cache = new ValidationCache();
        ExecutedRuleSets = EmptyRuleSetCollection;
        BrokenRules = Array.Empty<RuleFailure>();
    }

    public ValidationException(ValidationResult validationResult)
        : this(validationResult, message: null, innerException: null)
    {
    }

    public ValidationException(ValidationResult validationResult, string? message)
        : this(validationResult, message, innerException: null)
    {
    }

    public ValidationException(ValidationResult validationResult, string? message, Exception? innerException)
        : base(message ?? GetMessage(validationResult), innerException)
    {
        ArgumentNullException.ThrowIfNull(validationResult);

        Cache = validationResult.Cache;
        ExecutedRuleSets = validationResult.ExecutedRuleSets;
        BrokenRules = validationResult.BrokenRules;
    }

    public ValidationCache Cache { get; }

    public RuleSetCollection ExecutedRuleSets { get; }

    public IReadOnlyList<RuleFailure> BrokenRules { get; }

    private static string GetMessage(ValidationResult validationResult)
    {
        return $"Validation failed [Broken rules count: {validationResult.BrokenRules.Count}].";
    }
}
