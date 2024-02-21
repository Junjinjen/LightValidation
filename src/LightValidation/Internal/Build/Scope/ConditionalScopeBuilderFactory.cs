using LightValidation.Abstractions.Build;
using System;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Scope;

internal interface IConditionalScopeBuilderFactory
{
    IScopeBuilder<TEntity> Create<TEntity>(Func<ValidationContext<TEntity>, ValueTask<bool>> condition);
}

internal sealed class ConditionalScopeBuilderFactory : IConditionalScopeBuilderFactory
{
    public IScopeBuilder<TEntity> Create<TEntity>(Func<ValidationContext<TEntity>, ValueTask<bool>> condition)
    {
        return new ConditionalScopeBuilder<TEntity>(condition, DependencyResolver.ConditionalScopeFactory);
    }
}
