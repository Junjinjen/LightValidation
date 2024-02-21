using LightValidation.Abstractions.Execute;

namespace LightValidation.Internal.Execute.Scope;

internal interface IDependentScopeFactory
{
    IEntityValidator<TEntity> Create<TEntity>(IEntityValidator<TEntity>[] entityValidators);
}

internal sealed class DependentScopeFactory : IDependentScopeFactory
{
    public IEntityValidator<TEntity> Create<TEntity>(IEntityValidator<TEntity>[] entityValidators)
    {
        return new DependentScope<TEntity>(entityValidators);
    }
}
