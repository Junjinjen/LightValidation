using System;
using System.Collections.Generic;

namespace LightValidation.Rules;

public static class NotDefaultRule
{
    public static IValidator<TValue, TError> NotDefault<TValue, TError>(this IValidator<TValue, TError> validator, TError error)
    {
        ArgumentNullException.ThrowIfNull(validator);

        if (EqualityComparer<TValue>.Default.Equals(validator.Value, default))
        {
            validator.AddError(error);
        }

        return validator;
    }
}
