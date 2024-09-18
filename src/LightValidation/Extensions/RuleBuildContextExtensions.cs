using LightValidation.Abstractions.Build;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class RuleBuildContextExtensions
{
    public static Expression<Func<TEntity, TProperty>> GetPropertySelectorExpression<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return (Expression<Func<TEntity, TProperty>>)context.PropertySelectorExpression;
    }

    public static Func<TEntity, TProperty> GetPropertySelector<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return (Func<TEntity, TProperty>)context.PropertySelector;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(condition);

        context.AddCondition(condition);

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context, Func<TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(condition);

        context.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(condition);

        context.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context, Func<TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(condition);

        context.AddCondition((_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> AppendCollectionIndexToPropertyName<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context, bool value = true)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.AppendCollectionIndexToPropertyName(value);

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithDefaultErrorCode<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context, string defaultErrorCode)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(defaultErrorCode);

        context.SetDefaultErrorCode(defaultErrorCode);

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithDefaultErrorDescription<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context, string defaultErrorDescription)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(defaultErrorDescription);

        context.SetDefaultErrorDescription(defaultErrorDescription);

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context, string key, object? value)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(key);

        context.AddErrorMetadata(key, value);

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleBuildContext<TEntity, TProperty> context, string key, TValue value, Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        context.AddErrorMetadata(key, value);
        context.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context,
        string key,
        Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        context.AddErrorMetadata(key, valueSelector);

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleBuildContext<TEntity, TProperty> context,
        string key,
        Func<ValidationContext<TEntity>, TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        context.AddErrorMetadata(key, valueSelector);
        context.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context, string key, Func<TProperty, object?> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        context.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleBuildContext<TEntity, TProperty> context,
        string key,
        Func<TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        context.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));
        context.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return context;
    }

    public static IRuleBuildContext<TEntity, TProperty> WithErrorMetadataLocalization<TEntity, TProperty>(
        this IRuleBuildContext<TEntity, TProperty> context, string key, Func<object?, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        context.SetErrorMetadataLocalization(key, localizer);

        return context;
    }
}
