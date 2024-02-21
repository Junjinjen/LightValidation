using LightValidation.Abstractions.Build;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class RuleBuildContextExtensions
{
    public static Expression<Func<TEntity, TProperty>> GetPropertySelectorExpression<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);

        return (Expression<Func<TEntity, TProperty>>)ruleBuildContext.PropertySelectorExpression;
    }

    public static Func<TEntity, TProperty> GetPropertySelector<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);

        return (Func<TEntity, TProperty>)ruleBuildContext.PropertySelector;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentNullException.ThrowIfNull(condition);

        ruleBuildContext.AddCondition(condition);

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext, Func<TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentNullException.ThrowIfNull(condition);

        ruleBuildContext.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentNullException.ThrowIfNull(condition);

        ruleBuildContext.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext, Func<TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentNullException.ThrowIfNull(condition);

        ruleBuildContext.AddCondition((_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithDefaultErrorCode<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext, string defaultErrorCode)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(defaultErrorCode);

        ruleBuildContext.SetDefaultErrorCode(defaultErrorCode);

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithDefaultErrorDescription<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext, string defaultErrorDescription)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(defaultErrorDescription);

        ruleBuildContext.SetDefaultErrorDescription(defaultErrorDescription);

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext, string key, object? value)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(key);

        ruleBuildContext.AddErrorMetadata(key, value);

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext,
        string key,
        TValue value,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleBuildContext.AddErrorMetadata(key, value);
        ruleBuildContext.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext,
        string key,
        Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        ruleBuildContext.AddErrorMetadata(key, valueSelector);

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext,
        string key,
        Func<ValidationContext<TEntity>, TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleBuildContext.AddErrorMetadata(key, valueSelector);
        ruleBuildContext.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext,
        string key,
        Func<TProperty, object?> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        ruleBuildContext.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext,
        string key,
        Func<TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleBuildContext.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));
        ruleBuildContext.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleBuildContext;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithMetadataLocalization<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> ruleBuildContext, string key, Func<object?, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(ruleBuildContext);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleBuildContext.SetMetadataLocalization(key, localizer);

        return ruleBuildContext;
    }
}
