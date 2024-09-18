using LightValidation.Abstractions.Build;
using System;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class NestedValidatorConfigurationExtensions
{
    public static INestedValidatorConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this INestedValidatorConfiguration<TEntity, TProperty> configuration,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition(condition);

        return configuration;
    }

    public static INestedValidatorConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this INestedValidatorConfiguration<TEntity, TProperty> configuration,
        Func<TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return configuration;
    }

    public static INestedValidatorConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this INestedValidatorConfiguration<TEntity, TProperty> configuration,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return configuration;
    }

    public static INestedValidatorConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this INestedValidatorConfiguration<TEntity, TProperty> configuration, Func<TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(condition);

        configuration.AddCondition((_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return configuration;
    }

    public static INestedValidatorConfiguration<TEntity, TProperty> WithPropertyName<TEntity, TProperty>(
        this INestedValidatorConfiguration<TEntity, TProperty> configuration, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        configuration.SetPropertyName(propertyName);

        return configuration;
    }
}
