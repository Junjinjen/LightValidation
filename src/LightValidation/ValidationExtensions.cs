using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace LightValidation;

public static class ValidationExtensions
{
    public static IValidator<TModel, TError> Validate<TModel, TError>(
        this IValidationContext<TError> context, TModel model, [CallerArgumentExpression(nameof(model))] string modelName = "")
    {
        ArgumentNullException.ThrowIfNull(context);

        return new Validator<TModel, TError>(model)
        {
            Context = context,
            PropertyPath = modelName,
        };
    }

    public static IValidator<TProperty, TError> Property<TModel, TProperty, TError>(
        this IValidator<TModel, TError> validator, Expression<Func<TModel, TProperty>> selectorExpression)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(selectorExpression);

        return validator.Property<TProperty>(selectorExpression);
    }
}
