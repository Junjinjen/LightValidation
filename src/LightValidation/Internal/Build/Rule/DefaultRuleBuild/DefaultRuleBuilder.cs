using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;

namespace LightValidation.Internal.Build.Rule.DefaultRuleBuild;

internal sealed class DefaultRuleBuilder<TEntity, TProperty> : IRuleBuilder<TEntity, TProperty>
{
    private readonly IRule<TEntity, TProperty> _rule;

    public DefaultRuleBuilder(IRule<TEntity, TProperty> rule)
    {
        _rule = rule;
    }

    public IPropertyRule<TEntity, TProperty> Build(IRuleBuildContext<TEntity, TProperty> context)
    {
        _rule.Configure(context);

        return _rule;
    }
}
