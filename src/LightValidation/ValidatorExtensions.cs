using LightValidation.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LightValidation;

public static class ValidatorExtensions
{
    public static IValidator<TProperty, TError> Property<TModel, TProperty, TError>(
        this IValidator<TModel, TError> validator, Expression<Func<TModel, TProperty>> selectorExpression)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(selectorExpression);

        return validator.Property<TProperty>(selectorExpression);
    }

    public static IValidator<IReadOnlyList<TValue>, TError> ForEach<TValue, TError>(
        this IValidator<IReadOnlyList<TValue>, TError> validator, Action<IValidator<TValue, TError>> validationAction)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(validationAction);

        for (var i = 0; i < validator.Value.Count; i++)
        {
            var propertyValidator = validator.Property(x => x[i]);
            validationAction.Invoke(propertyValidator);
        }

        return validator;
    }

    [OverloadResolutionPriority(1)]
    public static IValidator<IList<TValue>, TError> ForEach<TValue, TError>(
        this IValidator<IList<TValue>, TError> validator, Action<IValidator<TValue, TError>> validationAction)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(validationAction);

        for (var i = 0; i < validator.Value.Count; i++)
        {
            var propertyValidator = validator.Property(x => x[i]);
            validationAction.Invoke(propertyValidator);
        }

        return validator;
    }

    public static IValidator<TValue, TError> EnsureContextValid<TValue, TError>(this IValidator<TValue, TError> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);

        validator.Context.EnsureValid();

        return validator;
    }

    public static IValidator<TValue, TError> IfValid<TValue, TError>(
        this IValidator<TValue, TError> validator, Action<IValidator<TValue, TError>> validationAction)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(validationAction);

        if (validator.IsValid)
        {
            validationAction.Invoke(validator);
        }

        return validator;
    }

    public static TResult? IfValid<TValue, TError, TResult>(
        this IValidator<TValue, TError> validator, Func<IValidator<TValue, TError>, TResult?> validationAction)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(validationAction);

        return validator.IsValid ? validationAction.Invoke(validator) : default;
    }

    public static Task<TResult?> IfValid<TValue, TError, TResult>(
        this IValidator<TValue, TError> validator, Func<IValidator<TValue, TError>, Task<TResult?>> validationAction)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(validationAction);

        return validator.IsValid ? validationAction.Invoke(validator) : Task.FromResult<TResult?>(default);
    }

    public static string GetPropertyName<TError>(this IValidator<TError> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);

        var nameResolver = validator.Context.ServiceProvider.GetService(typeof(IPropertyNameResolver)) as IPropertyNameResolver
            ?? new PropertyNameResolver();

        return PropertyNameBuilder.Build(validator.PathExpression, nameResolver);
    }
}
