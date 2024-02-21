using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Chain;

internal interface ICollectionRuleChainFactory
{
    IPropertyValidator<TEntity, IEnumerable<TProperty>?> Create<TEntity, TProperty>(
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? condition,
        IPropertyValidator<TEntity, TProperty>[] propertyValidators,
        int metadataId);
}

internal sealed class CollectionRuleChainFactory : ICollectionRuleChainFactory
{
    public IPropertyValidator<TEntity, IEnumerable<TProperty>?> Create<TEntity, TProperty>(
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? condition,
        IPropertyValidator<TEntity, TProperty>[] propertyValidators,
        int metadataId)
    {
        return new CollectionRuleChain<TEntity, TProperty>(
            condition, propertyValidators, metadataId, DependencyResolver.ElementContextFactory);
    }
}
