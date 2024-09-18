using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using System;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class RuleChainConfigurationExtensions
{
    public static IRuleChainConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleChainConfiguration<TEntity, TProperty> configuration,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition(condition);

        return configuration;
    }

    public static IRuleChainConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleChainConfiguration<TEntity, TProperty> configuration, Func<TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return configuration;
    }

    public static IRuleChainConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleChainConfiguration<TEntity, TProperty> configuration,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return configuration;
    }

    public static IRuleChainConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleChainConfiguration<TEntity, TProperty> configuration, Func<TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition((_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return configuration;
    }

    public static IRuleChainConfiguration<TEntity, TProperty> WithExecutionModeForAttribute<TEntity, TProperty>(
        this IRuleChainConfiguration<TEntity, TProperty> configuration, Type attributeType, ExecutionMode mode)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        attributeType.EnsureAttributeType();
        mode.EnsureDefined();

        configuration.SetExecutionModeForAttribute(attributeType, mode);

        return configuration;
    }

    public static IRuleChainConfiguration<TEntity, TProperty> WithDefaultExecutionMode<TEntity, TProperty>(
        this IRuleChainConfiguration<TEntity, TProperty> configuration, ExecutionMode mode)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        mode.EnsureDefined();

        configuration.SetDefaultExecutionMode(mode);

        return configuration;
    }

    public static IRuleChainConfiguration<TEntity, TProperty> WithPropertyName<TEntity, TProperty>(
        this IRuleChainConfiguration<TEntity, TProperty> configuration, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        configuration.SetPropertyName(propertyName);

        return configuration;
    }
}
