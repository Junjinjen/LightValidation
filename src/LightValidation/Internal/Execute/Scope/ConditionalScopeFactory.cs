using LightValidation.Abstractions.Execute;
using System;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Scope;

internal interface IConditionalScopeFactory
{
    IEntityValidator<TEntity> Create<TEntity>(
        Func<ValidationContext<TEntity>, ValueTask<bool>> condition,
        IEntityValidator<TEntity>[] entityValidators,
        int metadataId);
}

internal sealed class ConditionalScopeFactory : IConditionalScopeFactory
{
    public IEntityValidator<TEntity> Create<TEntity>(
        Func<ValidationContext<TEntity>, ValueTask<bool>> condition,
        IEntityValidator<TEntity>[] entityValidators,
        int metadataId)
    {
        return new ConditionalScope<TEntity>(condition, entityValidators, metadataId);
    }
}
