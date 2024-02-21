using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal;
using LightValidation.Internal.Build.Extensions;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightValidation.Extensions;

public static class ValidationBuilderExtensions
{
    public static IValidationBuilder<TEntity> AllowNullEntity<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);

        validationBuilder.AllowNullEntity(value: true);

        return validationBuilder;
    }

    public static IValidationBuilder<TEntity> ForbidNullEntity<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);

        validationBuilder.AllowNullEntity(value: false);

        return validationBuilder;
    }

    public static IValidationBuilder<TEntity> WithNullEntityErrorCode<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder, string errorCode)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        ArgumentException.ThrowIfNullOrEmpty(errorCode);

        validationBuilder.SetNullEntityErrorCode(errorCode);

        return validationBuilder;
    }

    public static IValidationBuilder<TEntity> WithNullEntityErrorDescription<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder, string errorDescription)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        ArgumentException.ThrowIfNullOrEmpty(errorDescription);

        validationBuilder.SetNullEntityErrorDescription(errorDescription);

        return validationBuilder;
    }

    public static IValidationBuilder<TEntity> WithExecutionModeForAttribute<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder, Type attributeType, ExecutionMode mode)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        attributeType.EnsureAttributeType();
        mode.EnsureDefined();

        validationBuilder.SetExecutionModeForAttribute(attributeType, mode);

        return validationBuilder;
    }

    public static IValidationBuilder<TEntity> WithDefaultExecutionMode<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder, ExecutionMode mode)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        mode.EnsureDefined();

        validationBuilder.SetDefaultExecutionMode(mode);

        return validationBuilder;
    }

    public static IValidationBuilder<TEntity> WithPropertyName<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder, string propertyPath, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        ArgumentException.ThrowIfNullOrEmpty(propertyPath);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        validationBuilder.SetPropertyName(propertyPath, propertyName);

        return validationBuilder;
    }

    public static IValidationBuilder<TEntity> WithPropertyName<TEntity, TProperty>(
        this IValidationBuilder<TEntity> validationBuilder,
        Expression<Func<TEntity, TProperty>> propertySelectorExpression,
        string propertyName)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        var propertyPath = propertySelectorExpression.EnsureValidPropertySelectorExpression();
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        validationBuilder.SetPropertyName(propertyPath, propertyName);

        return validationBuilder;
    }

    public static IRuleChainBuilderInitial<TEntity, TProperty> Property<TEntity, TProperty>(
        this IValidationBuilder<TEntity> validationBuilder,
        Expression<Func<TEntity, TProperty>> propertySelectorExpression)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        ArgumentNullException.ThrowIfNull(propertySelectorExpression);

        var ruleChainBuilderFactory = DependencyResolver.RuleChainBuilderFactory;
        var ruleChainBuilder = ruleChainBuilderFactory.Create<TEntity, TProperty>(validationBuilder);

        validationBuilder.AddPropertyValidator(propertySelectorExpression, ruleChainBuilder);

        return ruleChainBuilder;
    }

    public static void When<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder,
        Func<ValidationContext<TEntity>, ValueTask<bool>> condition,
        Action buildAction)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(buildAction);

        var conditionalScopeBuilderFactory = DependencyResolver.ConditionalScopeBuilderFactory;
        var conditionalScopeBuilder = conditionalScopeBuilderFactory.Create(condition);

        validationBuilder.AddScope(conditionalScopeBuilder, standaloneMode: false, buildAction);
    }

    public static void When<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder,
        Func<TEntity, ValueTask<bool>> condition,
        Action buildAction)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(buildAction);

        var conditionalScopeBuilderFactory = DependencyResolver.ConditionalScopeBuilderFactory;
        var conditionalScopeBuilder = conditionalScopeBuilderFactory.Create<TEntity>(
            context => condition.Invoke(context.Entity));

        validationBuilder.AddScope(conditionalScopeBuilder, standaloneMode: false, buildAction);
    }

    public static void When<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder,
        Func<ValidationContext<TEntity>, bool> condition,
        Action buildAction)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(buildAction);

        var conditionalScopeBuilderFactory = DependencyResolver.ConditionalScopeBuilderFactory;
        var conditionalScopeBuilder = conditionalScopeBuilderFactory.Create<TEntity>(
            context => ValueTask.FromResult(condition.Invoke(context)));

        validationBuilder.AddScope(conditionalScopeBuilder, standaloneMode: false, buildAction);
    }

    public static void When<TEntity>(
        this IValidationBuilder<TEntity> validationBuilder,
        Func<TEntity, bool> condition,
        Action buildAction)
    {
        ArgumentNullException.ThrowIfNull(validationBuilder);
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(buildAction);

        var conditionalScopeBuilderFactory = DependencyResolver.ConditionalScopeBuilderFactory;
        var conditionalScopeBuilder = conditionalScopeBuilderFactory.Create<TEntity>(
            context => ValueTask.FromResult(condition.Invoke(context.Entity)));

        validationBuilder.AddScope(conditionalScopeBuilder, standaloneMode: false, buildAction);
    }
}
