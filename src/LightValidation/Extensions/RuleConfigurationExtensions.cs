using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using System;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class RuleConfigurationExtensions
{
    public static IRuleConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition(condition);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithCondition<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition(condition);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, Func<TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithCondition<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, Func<TProperty, ValueTask<bool>> condition)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithCondition<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, Func<TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition((_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithCondition<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, Func<TProperty, bool> condition)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition((_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithExecutionMode<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, ExecutionMode mode)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        mode.EnsureDefined();

        configuration.SetExecutionMode(mode);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithExecutionMode<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, ExecutionMode mode)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        mode.EnsureDefined();

        configuration.SetExecutionMode(mode);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithPropertyName<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        configuration.SetPropertyName(propertyName);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithPropertyName<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, string propertyName)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        configuration.SetPropertyName(propertyName);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> AppendCollectionIndexToPropertyName<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, bool value = true)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.AppendCollectionIndexToPropertyName(value);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> AppendCollectionIndexToPropertyName
        <TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, bool value = true)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.AppendCollectionIndexToPropertyName(value);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorCode<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, string errorCode)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(errorCode);

        configuration.SetErrorCode(errorCode);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorCode<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, string errorCode)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(errorCode);

        configuration.SetErrorCode(errorCode);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorDescription<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, string errorDescription)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(errorDescription);

        configuration.SetErrorDescription(errorDescription);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorDescription<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, string errorDescription)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(errorDescription);

        configuration.SetErrorDescription(errorDescription);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, string key, object? value)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);

        configuration.AddErrorMetadata(key, value);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, string key, object? value)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);

        configuration.AddErrorMetadata(key, value);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleConfiguration<TEntity, TProperty> configuration,
        string key,
        TValue value,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        configuration.AddErrorMetadata(key, value);
        configuration.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TValue, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration,
        string key,
        TValue value,
        Func<TValue, string> localizer)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        configuration.AddErrorMetadata(key, value);
        configuration.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration,
        string key,
        Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        configuration.AddErrorMetadata(key, valueSelector);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration,
        string key,
        Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        configuration.AddErrorMetadata(key, valueSelector);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleConfiguration<TEntity, TProperty> configuration,
        string key,
        Func<ValidationContext<TEntity>, TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        configuration.AddErrorMetadata(key, valueSelector);
        configuration.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TValue, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration,
        string key,
        Func<ValidationContext<TEntity>, TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        configuration.AddErrorMetadata(key, valueSelector);
        configuration.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration,
        string key,
        Func<TProperty, object?> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        configuration.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration,
        string key,
        Func<TProperty, object?> valueSelector)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        configuration.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleConfiguration<TEntity, TProperty> configuration,
        string key,
        Func<TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        configuration.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));
        configuration.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TValue, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration,
        string key,
        Func<TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        configuration.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));
        configuration.SetErrorMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadataLocalization<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, string key, Func<object?, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        configuration.SetErrorMetadataLocalization(key, localizer);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadataLocalization
        <TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration,
        string key,
        Func<object?, string> localizer)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        configuration.SetErrorMetadataLocalization(key, localizer);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithDependentRules<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> configuration, Action buildAction)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(buildAction);

        configuration.AddDependentRules(buildAction);

        return configuration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithDependentRules<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> configuration, Action buildAction)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(buildAction);

        configuration.AddDependentRules(buildAction);

        return configuration;
    }
}
