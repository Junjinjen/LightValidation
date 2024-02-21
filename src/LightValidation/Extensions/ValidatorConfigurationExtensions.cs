using LightValidation.Abstractions.Build;
using System;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class ValidatorConfigurationExtensions
{
    public static IValidatorConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IValidatorConfiguration<TEntity, TProperty> validatorConfiguration,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(validatorConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        validatorConfiguration.AddCondition(condition);

        return validatorConfiguration;
    }

    public static IValidatorConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IValidatorConfiguration<TEntity, TProperty> validatorConfiguration,
        Func<TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(validatorConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        validatorConfiguration.AddCondition((_, propertyValue) => condition.Invoke(propertyValue));

        return validatorConfiguration;
    }

    public static IValidatorConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IValidatorConfiguration<TEntity, TProperty> validatorConfiguration,
        Func<ValidationContext<TEntity>, TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(validatorConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        validatorConfiguration.AddCondition(
            (context, propertyValue) => ValueTask.FromResult(condition.Invoke(context, propertyValue)));

        return validatorConfiguration;
    }

    public static IValidatorConfiguration<TEntity, TProperty> WithCondition<TEntity, TProperty>(
        this IValidatorConfiguration<TEntity, TProperty> validatorConfiguration, Func<TProperty, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(validatorConfiguration);
        ArgumentNullException.ThrowIfNull(condition);

        validatorConfiguration.AddCondition(
            (_, propertyValue) => ValueTask.FromResult(condition.Invoke(propertyValue)));

        return validatorConfiguration;
    }

    public static IValidatorConfiguration<TEntity, TProperty> WithPropertyName<TEntity, TProperty>(
        this IValidatorConfiguration<TEntity, TProperty> validatorConfiguration, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(validatorConfiguration);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        validatorConfiguration.SetPropertyName(propertyName);

        return validatorConfiguration;
    }
}
