using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightValidation.Rules;

public static class NullabilityRule
{
    private static readonly ConcurrentDictionary<Type, Delegate> DelegateCache = [];

    public static IValidator<TValue, TError> CheckNullability<TValue, TError>(
        this IValidator<TValue, TError> validator, Action<IValidator<TError>> errorCallback)
        where TValue : class?
    {
        return CheckNullability(validator, allowDefaultValueTypes: true, errorCallback);
    }

    public static IValidator<TValue, TError> CheckNullability<TValue, TError>(
        this IValidator<TValue, TError> validator, bool allowDefaultValueTypes, Action<IValidator<TError>> errorCallback)
        where TValue : class?
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(errorCallback);

        if (validator.Value == null)
        {
            errorCallback.Invoke(validator);

            return validator;
        }

        var validatorType = typeof(IValidator<TValue, TError>);
        var lambda = DelegateCache.GetOrAdd(validatorType, validatorType =>
        {
            var nullabilityContext = new NullabilityInfoContext();
            var errorCallbackParameter = Expression.Parameter(typeof(Action<IValidator<TError>>), nameof(errorCallback));
            var validatorParameter = Expression.Parameter(validatorType, nameof(validator));
            var valueParameter = Expression.Parameter(typeof(TValue), "x");

            var validationExpressions = typeof(TValue)
                .GetProperties()
                .Where(x => !x.PropertyType.IsValueType || !allowDefaultValueTypes)
                .Where(x => nullabilityContext.Create(x).ReadState != NullabilityState.Nullable)
                .Select(x =>
                {
                    var selectorBody = Expression.MakeMemberAccess(valueParameter, x);
                    var selector = Expression.Lambda(selectorBody, valueParameter);
                    var propertyCall = Expression.Call(
                        validatorParameter, nameof(IValidator<TValue, TError>.Property), [x.PropertyType], selector);

                    return Expression.Call(
                        typeof(NullabilityRule),
                        nameof(ValidateNullability),
                        [x.PropertyType, typeof(TError)],
                        propertyCall,
                        errorCallbackParameter);
                });

            var body = Expression.Block(validationExpressions);
            var lambdaExpression = Expression.Lambda<Action<IValidator<TValue, TError>, Action<IValidator<TError>>>>(
                body, validatorParameter, errorCallbackParameter);

            return lambdaExpression.Compile();
        });

        var typed = (Action<IValidator<TValue, TError>, Action<IValidator<TError>>>)lambda;
        typed.Invoke(validator, errorCallback);

        return validator;
    }

    private static void ValidateNullability<TValue, TError>(
        IValidator<TValue, TError> validator, Action<IValidator<TError>> errorCallback)
    {
        if (EqualityComparer<TValue>.Default.Equals(validator.Value, default))
        {
            errorCallback.Invoke(validator);
        }
    }
}
