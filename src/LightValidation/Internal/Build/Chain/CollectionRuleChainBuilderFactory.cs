using LightValidation.Abstractions.Build;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Property.Context;
using System.Collections.Generic;

namespace LightValidation.Internal.Build.Chain;

internal interface ICollectionRuleChainBuilderFactory
{
    ICollectionRuleChainBuilder<TEntity, TProperty> Create<TEntity, TProperty>(
        IRuleChainBuilder<TEntity, IEnumerable<TProperty>> ruleChainBuilder);
}

internal sealed class CollectionRuleChainBuilderFactory : ICollectionRuleChainBuilderFactory
{
    public ICollectionRuleChainBuilder<TEntity, TProperty> Create<TEntity, TProperty>(
        IRuleChainBuilder<TEntity, IEnumerable<TProperty>> ruleChainBuilder)
    {
        var propertyConditionBuilder = new PropertyConditionBuilder<TEntity, TProperty>();
        var propertyContextEditor = new PropertyContextEditor(DependencyResolver.PropertyBuildContextFactory);

        return new CollectionRuleChainBuilder<TEntity, TProperty>(
            propertyConditionBuilder,
            propertyContextEditor,
            ruleChainBuilder,
            DependencyResolver.CollectionRuleChainFactory);
    }
}
