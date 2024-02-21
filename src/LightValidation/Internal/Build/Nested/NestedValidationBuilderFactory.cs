using LightValidation.Abstractions.Build;
using LightValidation.Internal.Build.Property;
using System;

namespace LightValidation.Internal.Build.Nested;

internal interface INestedValidationBuilderFactory
{
    INestedValidationBuilder<TEntity, TProperty> Create<TEntity, TProperty>(
        Func<ValidationContext<TEntity>, IValidatorInternal<TProperty>> validatorProvider,
        IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder);
}

internal sealed class NestedValidationBuilderFactory : INestedValidationBuilderFactory
{
    public INestedValidationBuilder<TEntity, TProperty> Create<TEntity, TProperty>(
        Func<ValidationContext<TEntity>, IValidatorInternal<TProperty>> validatorProvider,
        IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder)
    {
        var propertyConditionBuilder = new PropertyConditionBuilder<TEntity, TProperty>();

        return new NestedValidationBuilder<TEntity, TProperty>(
            validatorProvider,
            propertyConditionBuilder,
            DependencyResolver.NestedValidationExecutorFactory,
            ruleChainBuilder);
    }
}
