using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Options = LightValidation.Rules.NullabilityCheckOptions;

namespace LightValidation.Rules;

public static class NullabilityRule
{
    private static readonly ConcurrentDictionary<Type, PropertyValidator[]> ValidationCache = [];

    public static IValidator<TModel, TError> CheckNullability<TModel, TError>(
        this IValidator<TModel, TError> validator, Action<IValidator<TError>> errorCallback)
    {
        return CheckNullability(validator, errorCallback, Options.Default);
    }

    public static IValidator<TModel, TError> CheckNullability<TModel, TError>(
        this IValidator<TModel, TError> validator, Action<IValidator<TError>> errorCallback, Options options)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(errorCallback);
        ArgumentNullException.ThrowIfNull(options);

        if (validator.Value == null!)
        {
            errorCallback.Invoke(validator);

            return validator;
        }

        var validatorType = typeof(IValidator<TModel, TError>);
        var propertyValidators = ValidationCache.GetOrAdd(validatorType, validatorType =>
        {
            var validatorParameter = Expression.Parameter(validatorType, nameof(validator));
            var errorCallbackParameter = Expression.Parameter(typeof(Action<IValidator<TError>>), nameof(errorCallback));
            var optionsParameter = Expression.Parameter(typeof(Options), nameof(options));
            var modelParameter = Expression.Parameter(typeof(TModel), "model");
            var nullabilityContext = new NullabilityInfoContext();

            return typeof(TModel)
                .GetProperties()
                .Select(x => PropertyValidator.Create<TModel, TError>(
                    x, validatorParameter, errorCallbackParameter, optionsParameter, modelParameter, nullabilityContext))
                .ToArray();
        });

        foreach (var propertyValidator in propertyValidators)
        {
            propertyValidator.Validate(validator, errorCallback, options);
        }

        return validator;
    }

    private sealed class PropertyValidator
    {
        private readonly Delegate _validationDelegate;
        private readonly bool _isNullableReferenceType;
        private readonly bool _isValueType;

        private PropertyValidator(Delegate validationDelegate, bool isNullableReferenceType, bool isValueType)
        {
            _validationDelegate = validationDelegate;
            _isNullableReferenceType = isNullableReferenceType;
            _isValueType = isValueType;
        }

        public static PropertyValidator Create<TModel, TError>(
            PropertyInfo propertyInfo,
            ParameterExpression validatorParameter,
            ParameterExpression errorCallbackParameter,
            ParameterExpression optionsParameter,
            ParameterExpression modelParameter,
            NullabilityInfoContext nullabilityContext)
        {
            var propertyType = propertyInfo.PropertyType;
            var selectorBody = Expression.MakeMemberAccess(modelParameter, propertyInfo);
            var selector = Expression.Lambda(selectorBody, modelParameter);
            var propertyCall = Expression.Call(validatorParameter, nameof(IValidator<,>.Property), [propertyType], selector);

            var isStringType = propertyType == typeof(string);
            var methodName = isStringType ? nameof(StringValidationLogic) : nameof(ValidationLogic);
            Type[] typeParameters = isStringType ? [typeof(TError)] : [propertyType, typeof(TError)];
            var body = Expression.Call(
                typeof(PropertyValidator), methodName, typeParameters, propertyCall, errorCallbackParameter, optionsParameter);

            var lambda = Expression.Lambda<Action<IValidator<TModel, TError>, Action<IValidator<TError>>, Options>>(
                body, validatorParameter, errorCallbackParameter, optionsParameter);

            var validationDelegate = lambda.Compile();
            var isValueType = propertyType.IsValueType;
            var isNullableReferenceType = !isValueType
                && nullabilityContext.Create(propertyInfo).ReadState == NullabilityState.Nullable;

            return new PropertyValidator(validationDelegate, isNullableReferenceType, isValueType);
        }

        public void Validate<TModel, TError>(
            IValidator<TModel, TError> validator, Action<IValidator<TError>> errorCallback, Options options)
        {
            if (_isNullableReferenceType && !options.CheckNullableReferenceTypes)
            {
                return;
            }

            if (_isValueType && options.AllowDefaultValueTypes)
            {
                return;
            }

            var typed = (Action<IValidator<TModel, TError>, Action<IValidator<TError>>, Options>)_validationDelegate;
            typed.Invoke(validator, errorCallback, options);
        }

        private static void ValidationLogic<TValue, TError>(
            IValidator<TValue, TError> validator, Action<IValidator<TError>> errorCallback, Options options)
        {
            if (!options.CheckInvalidProperties && !validator.IsValid)
            {
                return;
            }

            if (EqualityComparer<TValue>.Default.Equals(validator.Value, default))
            {
                errorCallback.Invoke(validator);
            }
        }

        private static void StringValidationLogic<TError>(
            IValidator<string, TError> validator, Action<IValidator<TError>> errorCallback, Options options)
        {
            if (!options.CheckInvalidProperties && !validator.IsValid)
            {
                return;
            }

            var result = options.AllowEmptyStrings ? validator.Value != null : !string.IsNullOrEmpty(validator.Value);
            if (!result)
            {
                errorCallback.Invoke(validator);
            }
        }
    }
}
