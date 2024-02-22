using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using System;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class RuleConfigurationExtensions
{
    public static IRuleConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        ruleConfiguration.AddCondition(condition);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithCondition<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        ruleConfiguration.AddCondition(condition);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, Func<TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        ruleConfiguration.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithCondition<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration,
        Func<TProperty, ValueTask<bool>> condition)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        ruleConfiguration.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        ruleConfiguration.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithCondition<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        ruleConfiguration.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, Func<TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        ruleConfiguration.AddCondition(
            (_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithCondition<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration, Func<TProperty, bool> condition)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        ruleConfiguration.AddCondition(
            (_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithExecutionMode<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, ExecutionMode mode)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        mode.EnsureDefined();

        ruleConfiguration.SetExecutionMode(mode);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithExecutionMode<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration, ExecutionMode mode)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        mode.EnsureDefined();

        ruleConfiguration.SetExecutionMode(mode);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithPropertyName<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        ruleConfiguration.SetPropertyName(propertyName);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithPropertyName<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration, string propertyName)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        ruleConfiguration.SetPropertyName(propertyName);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithIndexOnPropertyName<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);

        ruleConfiguration.ApplyIndexOnPropertyName(value: true);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithIndexOnPropertyName<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);

        ruleConfiguration.ApplyIndexOnPropertyName(value: true);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithoutIndexOnPropertyName<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);

        ruleConfiguration.ApplyIndexOnPropertyName(value: false);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithoutIndexOnPropertyName<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);

        ruleConfiguration.ApplyIndexOnPropertyName(value: false);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorCode<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, string errorCode)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(errorCode);

        ruleConfiguration.SetErrorCode(errorCode);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorCode<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration, string errorCode)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(errorCode);

        ruleConfiguration.SetErrorCode(errorCode);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorDescription<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, string errorDescription)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(errorDescription);

        ruleConfiguration.SetErrorDescription(errorDescription);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorDescription<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration, string errorDescription)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(errorDescription);

        ruleConfiguration.SetErrorDescription(errorDescription);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, string key, object? value)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);

        ruleConfiguration.AddErrorMetadata(key, value);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration, string key, object? value)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);

        ruleConfiguration.AddErrorMetadata(key, value);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration,
        string key,
        TValue value,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleConfiguration.AddErrorMetadata(key, value);
        ruleConfiguration.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TValue, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration,
        string key,
        TValue value,
        Func<TValue, string> localizer)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleConfiguration.AddErrorMetadata(key, value);
        ruleConfiguration.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration,
        string key,
        Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        ruleConfiguration.AddErrorMetadata(key, valueSelector);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration,
        string key,
        Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        ruleConfiguration.AddErrorMetadata(key, valueSelector);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration,
        string key,
        Func<ValidationContext<TEntity>, TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleConfiguration.AddErrorMetadata(key, valueSelector);
        ruleConfiguration.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TValue, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration,
        string key,
        Func<ValidationContext<TEntity>, TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleConfiguration.AddErrorMetadata(key, valueSelector);
        ruleConfiguration.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration,
        string key,
        Func<TProperty, object?> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        ruleConfiguration.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration,
        string key,
        Func<TProperty, object?> valueSelector)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);

        ruleConfiguration.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithErrorMetadata<TEntity, TProperty, TValue>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration,
        string key,
        Func<TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleConfiguration.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));
        ruleConfiguration.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithErrorMetadata<TEntity, TProperty, TValue, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration,
        string key,
        Func<TProperty, TValue> valueSelector,
        Func<TValue, string> localizer)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(valueSelector);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleConfiguration.AddErrorMetadata(key, (_, propertyValue) => valueSelector.Invoke(propertyValue));
        ruleConfiguration.SetMetadataLocalization(key, x => localizer.Invoke((TValue)x!));

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithMetadataLocalization<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, string key, Func<object?, string> localizer)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleConfiguration.SetMetadataLocalization(key, localizer);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithMetadataLocalization<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration, string key, Func<object?, string> localizer)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(localizer);

        ruleConfiguration.SetMetadataLocalization(key, localizer);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty> WithDependentRules<TEntity, TProperty>(
        this IRuleConfiguration<TEntity, TProperty> ruleConfiguration, Action buildAction)
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(buildAction);

        ruleConfiguration.AddDependentRules(buildAction);

        return ruleConfiguration;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> WithDependentRules<TEntity, TProperty, TRule>(
        this IRuleConfiguration<TEntity, TProperty, TRule> ruleConfiguration, Action buildAction)
        where TRule : notnull
    {
        ArgumentNullException.ThrowIfNull(ruleConfiguration);
        ArgumentNullException.ThrowIfNull(buildAction);

        ruleConfiguration.AddDependentRules(buildAction);

        return ruleConfiguration;
    }
}
