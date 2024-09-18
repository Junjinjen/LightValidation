using LightValidation.Abstractions.Build;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Rule.FailureGeneration;

namespace LightValidation.Internal.Build.Rule;

internal interface IRuleValidationBuilderFactory
{
    IRuleValidationBuilder<TEntity, TProperty, TRule> Create<TEntity, TProperty, TRule>(
        IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder,
        IRuleBuilder<TEntity, TProperty> ruleBuilder,
        TRule rule)
        where TRule : notnull;
}

internal sealed class RuleValidationBuilderFactory : IRuleValidationBuilderFactory
{
    public IRuleValidationBuilder<TEntity, TProperty, TRule> Create<TEntity, TProperty, TRule>(
        IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder,
        IRuleBuilder<TEntity, TProperty> ruleBuilder,
        TRule rule)
        where TRule : notnull
    {
        var defaultRuleConfiguration = new DefaultRuleConfiguration();
        var ruleFailureGeneratorBuilder = new RuleFailureGeneratorBuilder<TEntity, TProperty>(
            DependencyResolver.RuntimeDescriptionGeneratorFactory,
            DependencyResolver.StaticDescriptionGeneratorFactory,
            DependencyResolver.RuntimeMetadataGeneratorFactory,
            DependencyResolver.StaticMetadataGeneratorFactory,
            DependencyResolver.RuleFailureGeneratorFactory,
            DependencyResolver.ErrorMetadataProviderFactory,
            defaultRuleConfiguration);

        var propertyConditionBuilder = new PropertyConditionBuilder<TEntity, TProperty>();
        var executionModeProvider = new ExecutionModeProvider();

        return new RuleValidationBuilder<TEntity, TProperty, TRule>(
            ruleFailureGeneratorBuilder,
            propertyConditionBuilder,
            DependencyResolver.RuleValidationExecutorFactory,
            DependencyResolver.DependentScopeBuilderFactory,
            ruleChainBuilder,
            ruleBuilder,
            executionModeProvider,
            DependencyResolver.RuleContextFactory)
        {
            Rule = rule,
        };
    }
}
