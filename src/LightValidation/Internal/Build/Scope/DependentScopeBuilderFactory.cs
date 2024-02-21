using LightValidation.Abstractions.Build;

namespace LightValidation.Internal.Build.Scope;

internal interface IDependentScopeBuilderFactory
{
    IScopeBuilder<TEntity> Create<TEntity>();
}

internal sealed class DependentScopeBuilderFactory : IDependentScopeBuilderFactory
{
    public IScopeBuilder<TEntity> Create<TEntity>()
    {
        return new DependentScopeBuilder<TEntity>(DependencyResolver.DependentScopeFactory);
    }
}
