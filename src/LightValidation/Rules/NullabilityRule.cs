using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightValidation.Rules;

public static class NullabilityRule
{
    private static readonly ConcurrentDictionary<Type, Delegate> ValidationCache = [];

    public static IValidator<TModel, TError> CheckNullability<TModel, TError>(
        this IValidator<TModel, TError> validator, TError error)
    {
        ArgumentNullException.ThrowIfNull(validator);

        if (validator.Value == null!)
        {
            return validator.AddError(error);
        }

        var validation = ValidationCache.GetOrAdd(typeof(IValidator<TModel, TError>), validatorType =>
        {
            var validatorParameter = Expression.Parameter(validatorType);
            var modelParameter = Expression.Parameter(typeof(TModel));
            var errorParameter = Expression.Parameter(typeof(TError));

            var nullabilityContext = new NullabilityInfoContext();
            var validations = typeof(TModel)
                .GetProperties()
                .Where(x => !x.PropertyType.IsValueType && nullabilityContext.Create(x).ReadState != NullabilityState.Nullable)
                .Select(x =>
                {
                    var propertyType = x.PropertyType;
                    var selectorBody = Expression.MakeMemberAccess(modelParameter, x);
                    var selector = Expression.Lambda(selectorBody, modelParameter);
                    var propertyCall = Expression.Call(
                        validatorParameter, nameof(IValidator<,>.Property), [propertyType], selector);

                    return Expression.Call(
                        typeof(NullabilityRule),
                        nameof(ValidationLogic),
                        [propertyType, typeof(TError)],
                        propertyCall,
                        errorParameter);
                });

            var body = Expression.Block(validations);
            var lambda = Expression.Lambda(body, validatorParameter, errorParameter);

            return lambda.Compile();
        });

        var typed = (Action<IValidator<TModel, TError>, TError>)validation;
        typed.Invoke(validator, error);

        return validator;
    }

    private static void ValidationLogic<TValue, TError>(IValidator<TValue, TError> validator, TError error)
        where TValue : class?
    {
        if (validator.Value == null)
        {
            validator.AddError(error);
        }
    }
}
