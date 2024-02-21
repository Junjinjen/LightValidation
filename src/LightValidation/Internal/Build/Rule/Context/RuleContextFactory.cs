using LightValidation.Abstractions.Build;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Rule.FailureGeneration;

namespace LightValidation.Internal.Build.Rule.Context;

internal interface IRuleContextFactory
{
    IRuleBuildContext<TEntity, TProperty> Create<TEntity, TProperty>(
        IRuleFailureGeneratorBuilder<TEntity, TProperty> ruleFailureGeneratorBuilder,
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        IPropertyBuildContext propertyContext);
}

internal sealed class RuleContextFactory : IRuleContextFactory
{
    public IRuleBuildContext<TEntity, TProperty> Create<TEntity, TProperty>(
        IRuleFailureGeneratorBuilder<TEntity, TProperty> ruleFailureGeneratorBuilder,
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        IPropertyBuildContext propertyContext)
    {
        return new RuleContext<TEntity, TProperty>(
            ruleFailureGeneratorBuilder, propertyConditionBuilder, propertyContext);
    }
}
