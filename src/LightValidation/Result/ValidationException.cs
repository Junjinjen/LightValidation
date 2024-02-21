using LightValidation.Abstractions;
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
        ExecutedRuleSets = EmptyRuleSetCollection;
        BrokenRules = Array.Empty<RuleFailure>();
        ValidationCache = new ValidationCache();
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

        ExecutedRuleSets = validationResult.ExecutedRuleSets;
        BrokenRules = validationResult.BrokenRules;
        ValidationCache = validationResult.ValidationCache;
    }

    public RuleSetCollection ExecutedRuleSets { get; }

    public IReadOnlyList<RuleFailure> BrokenRules { get; }

    public IValidationCache ValidationCache { get; }

    private static string GetMessage(ValidationResult validationResult)
    {
        return $"Validation failed [Broken rules count: {validationResult.BrokenRules.Count}].";
    }
}
