using LightValidation.Abstractions.Build;

namespace LightValidation.Internal.Build.Rule.DefaultRuleBuild;

internal interface IDefaultRuleBuilderFactory
{
    IRuleBuilder<TEntity, TProperty> Create<TEntity, TProperty>(IRule<TEntity, TProperty> rule);
}

internal sealed class DefaultRuleBuilderFactory : IDefaultRuleBuilderFactory
{
    public IRuleBuilder<TEntity, TProperty> Create<TEntity, TProperty>(IRule<TEntity, TProperty> rule)
    {
        return new DefaultRuleBuilder<TEntity, TProperty>(rule);
    }
}
