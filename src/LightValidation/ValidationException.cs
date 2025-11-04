using System;
using System.Collections.Generic;
using System.Linq;

namespace LightValidation;

public abstract class ValidationException : Exception
{
    protected ValidationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public abstract IEnumerable<object?> GetErrors();
}

public sealed class ValidationException<TError> : ValidationException
{
    public ValidationException()
        : this([], message: null, innerException: null)
    {
    }

    public ValidationException(string? message)
        : this([], message, innerException: null)
    {
    }

    public ValidationException(string? message, Exception? innerException)
        : this([], message, innerException)
    {
    }

    public ValidationException(IReadOnlyList<TError> errors)
        : this(errors, message: null, innerException: null)
    {
    }

    public ValidationException(IReadOnlyList<TError> errors, string? message)
        : this(errors, message, innerException: null)
    {
    }

    public ValidationException(IReadOnlyList<TError> errors, string? message, Exception? innerException)
        : base(message ?? GetMessage(errors), innerException)
    {
        Errors = errors;
    }

    public IReadOnlyList<TError> Errors { get; }

    public override IEnumerable<object?> GetErrors()
    {
        return Errors.Cast<object?>();
    }

    private static string GetMessage(IReadOnlyList<TError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        return $"Validation failed. Number of errors: {errors.Count}.";
    }
}
