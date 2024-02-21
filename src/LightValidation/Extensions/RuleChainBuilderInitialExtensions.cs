using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using System;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class RuleChainBuilderInitialExtensions
{
    public static IRuleChainBuilderInitial<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleChainBuilderInitial<TEntity, TProperty> ruleChainBuilderInitial,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilderInitial);
        ArgumentNullException.ThrowIfNull(condition);

        ruleChainBuilderInitial.AddCondition(condition);

        return ruleChainBuilderInitial;
    }

    public static IRuleChainBuilderInitial<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleChainBuilderInitial<TEntity, TProperty> ruleChainBuilderInitial,
        Func<TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilderInitial);
        ArgumentNullException.ThrowIfNull(condition);

        ruleChainBuilderInitial.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return ruleChainBuilderInitial;
    }

    public static IRuleChainBuilderInitial<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleChainBuilderInitial<TEntity, TProperty> ruleChainBuilderInitial,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilderInitial);
        ArgumentNullException.ThrowIfNull(condition);

        ruleChainBuilderInitial.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return ruleChainBuilderInitial;
    }

    public static IRuleChainBuilderInitial<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IRuleChainBuilderInitial<TEntity, TProperty> ruleChainBuilderInitial, Func<TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilderInitial);
        ArgumentNullException.ThrowIfNull(condition);

        ruleChainBuilderInitial.AddCondition(
            (_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return ruleChainBuilderInitial;
    }

    public static IRuleChainBuilderInitial<TEntity, TProperty> WithExecutionModeForAttribute<TEntity, TProperty>(
        this IRuleChainBuilderInitial<TEntity, TProperty> ruleChainBuilderInitial, Type attributeType, ExecutionMode mode)
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilderInitial);
        attributeType.EnsureAttributeType();
        mode.EnsureDefined();

        ruleChainBuilderInitial.SetExecutionModeForAttribute(attributeType, mode);

        return ruleChainBuilderInitial;
    }

    public static IRuleChainBuilderInitial<TEntity, TProperty> WithDefaultExecutionMode<TEntity, TProperty>(
        this IRuleChainBuilderInitial<TEntity, TProperty> ruleChainBuilderInitial, ExecutionMode mode)
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilderInitial);
        mode.EnsureDefined();

        ruleChainBuilderInitial.SetDefaultExecutionMode(mode);

        return ruleChainBuilderInitial;
    }

    public static IRuleChainBuilderInitial<TEntity, TProperty> WithPropertyName<TEntity, TProperty>(
        this IRuleChainBuilderInitial<TEntity, TProperty> ruleChainBuilderInitial, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilderInitial);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        ruleChainBuilderInitial.SetPropertyName(propertyName);

        return ruleChainBuilderInitial;
    }
}
