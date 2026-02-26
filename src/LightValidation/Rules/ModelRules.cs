using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightValidation.Rules;

public static class ModelRules
{
    private static readonly ConcurrentDictionary<(Type ValidatorType, bool CheckValueTypes), Delegate> Cache = [];

    public static IValidator<TModel, TError> AllPropertiesNotDefault<TModel, TError>(
        this IValidator<TModel, TError> validator, TError error)
    {
        return CheckProperties(validator, checkValueTypes: true, error);
    }

    public static IValidator<TModel, TError> AllPropertiesNotNull<TModel, TError>(
        this IValidator<TModel, TError> validator, TError error)
    {
        return CheckProperties(validator, checkValueTypes: false, error);
    }

    private static IValidator<TModel, TError> CheckProperties<TModel, TError>(
        IValidator<TModel, TError> validator, bool checkValueTypes, TError error)
    {
        ArgumentNullException.ThrowIfNull(validator);

        if (validator.Value == null!)
        {
            return validator.AddError(error);
        }

        var key = (typeof(IValidator<TModel, TError>), checkValueTypes);
        var validation = Cache.GetOrAdd(key, key =>
        {
            var validatorParameter = Expression.Parameter(key.ValidatorType);
            var modelParameter = Expression.Parameter(typeof(TModel));
            var errorParameter = Expression.Parameter(typeof(TError));

            var nullabilityContext = new NullabilityInfoContext();
            var validations = typeof(TModel)
                .GetProperties()
                .Where(x => nullabilityContext.Create(x).ReadState != NullabilityState.Nullable)
                .Where(x => key.CheckValueTypes || !x.PropertyType.IsValueType)
                .Select(x =>
                {
                    var propertyType = x.PropertyType;
                    var selectorBody = Expression.MakeMemberAccess(modelParameter, x);
                    var selector = Expression.Lambda(selectorBody, modelParameter);
                    var propertyCall = Expression.Call(
                        validatorParameter, nameof(IValidator<,>.Property), [propertyType], selector);

                    return Expression.Call(
                        typeof(ModelRules),
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
    {
        if (EqualityComparer<TValue>.Default.Equals(validator.Value, default))
        {
            validator.AddError(error);
        }
    }
}
