using LightValidation.Abstractions.Build;
using LightValidation.Internal;
using System;
using System.Collections.Generic;

namespace LightValidation.Extensions;

public static class RuleChainBuilderExtensions
{
    public static IRuleConfiguration<TEntity, TProperty, TRule> AddRuleBuilder<TEntity, TProperty, TRule>(
        this IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder, TRule ruleBuilder)
        where TRule : notnull, IRuleBuilder<TEntity, TProperty>
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilder);
        ArgumentNullException.ThrowIfNull(ruleBuilder);

        var ruleValidationBuilderFactory = DependencyResolver.RuleValidationBuilderFactory;
        var ruleValidationBuilder = ruleValidationBuilderFactory.Create(ruleChainBuilder, ruleBuilder, ruleBuilder);

        ruleChainBuilder.AddPropertyValidator(ruleValidationBuilder);

        return ruleValidationBuilder;
    }

    public static IRuleConfiguration<TEntity, TProperty, TRule> AddRule<TEntity, TProperty, TRule>(
        this IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder, TRule rule)
        where TRule : notnull, IRule<TEntity, TProperty>
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilder);
        ArgumentNullException.ThrowIfNull(rule);

        var defaultRuleBuilderFactory = DependencyResolver.DefaultRuleBuilderFactory;
        var ruleValidationBuilderFactory = DependencyResolver.RuleValidationBuilderFactory;

        var ruleBuilder = defaultRuleBuilderFactory.Create(rule);
        var ruleValidationBuilder = ruleValidationBuilderFactory.Create(ruleChainBuilder, ruleBuilder, rule);

        ruleChainBuilder.AddPropertyValidator(ruleValidationBuilder);

        return ruleValidationBuilder;
    }

    public static ICollectionRuleChainConfiguration<TEntity, TProperty> ForEach<TEntity, TProperty>(
        this IRuleChainBuilder<TEntity, IEnumerable<TProperty>?> ruleChainBuilder,
        Action<IRuleChainConfiguration<TEntity, TProperty>> buildAction)
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilder);
        ArgumentNullException.ThrowIfNull(buildAction);

        var collectionRuleChainBuilderFactory = DependencyResolver.CollectionRuleChainBuilderFactory;
        var collectionRuleChainBuilder = collectionRuleChainBuilderFactory.Create(ruleChainBuilder!);

        ruleChainBuilder.AddPropertyValidator(collectionRuleChainBuilder);
        buildAction.Invoke(collectionRuleChainBuilder);

        return collectionRuleChainBuilder;
    }

    public static INestedValidatorConfiguration<TEntity, TProperty> UseValidator<TEntity, TProperty, TValidator>(
        this IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder,
        Func<ValidationContext<TEntity>, TValidator> validatorProvider)
        where TValidator : ValidatorBase<TProperty>
    {
        ArgumentNullException.ThrowIfNull(ruleChainBuilder);
        ArgumentNullException.ThrowIfNull(validatorProvider);

        var nestedValidationBuilderFactory = DependencyResolver.NestedValidationBuilderFactory;
        var nestedValidationBuilder = nestedValidationBuilderFactory.Create(validatorProvider, ruleChainBuilder);

        ruleChainBuilder.AddPropertyValidator(nestedValidationBuilder);

        return nestedValidationBuilder;
    }

    public static INestedValidatorConfiguration<TEntity, TProperty> UseValidator<TEntity, TProperty, TValidator>(
        this IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder, Of<TValidator>? _)
        where TValidator : ValidatorBase<TProperty>, new()
    {
        return ruleChainBuilder.UseValidator(_ => new TValidator());
    }
}
