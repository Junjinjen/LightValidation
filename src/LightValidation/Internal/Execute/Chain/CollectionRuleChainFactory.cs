using LightValidation.Abstractions.Execute;
using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Chain;

internal interface ICollectionRuleChainFactory
{
    IPropertyValidator<TEntity, IEnumerable<TProperty>?> Create<TEntity, TProperty>(
        in CollectionRuleChainParameters<TEntity, TProperty> parameters);
}

internal sealed class CollectionRuleChainFactory : ICollectionRuleChainFactory
{
    public IPropertyValidator<TEntity, IEnumerable<TProperty>?> Create<TEntity, TProperty>(
        in CollectionRuleChainParameters<TEntity, TProperty> parameters)
    {
        return new CollectionRuleChain<TEntity, TProperty>(
            parameters.Condition,
            parameters.PropertyValidators,
            parameters.MetadataId,
            parameters.IndexBuilder,
            DependencyResolver.ElementContextFactory);
    }
}
