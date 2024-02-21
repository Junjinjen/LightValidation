using LightValidation.Abstractions.Execute;
using System;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Chain;

internal interface IRuleChainFactory
{
    IPropertyValidator<TEntity, TProperty> Create<TEntity, TProperty>(
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? condition,
        IPropertyValidator<TEntity, TProperty>[] propertyValidators,
        int metadataId);
}

internal sealed class RuleChainFactory : IRuleChainFactory
{
    public IPropertyValidator<TEntity, TProperty> Create<TEntity, TProperty>(
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? condition,
        IPropertyValidator<TEntity, TProperty>[] propertyValidators,
        int metadataId)
    {
        return new RuleChain<TEntity, TProperty>(condition, propertyValidators, metadataId);
    }
}
