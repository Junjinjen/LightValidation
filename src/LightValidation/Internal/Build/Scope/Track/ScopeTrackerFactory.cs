using LightValidation.Abstractions.Build;
using System;

namespace LightValidation.Internal.Build.Scope.Track;

internal interface IScopeTrackerFactory
{
    IScopeTracker Create<TEntity>(
        Func<IScopeBuilder<TEntity>> scopeBuilderProvider, IScopeBuilder<TEntity> currentScopeBuilder);
}

internal sealed class ScopeTrackerFactory : IScopeTrackerFactory
{
    public IScopeTracker Create<TEntity>(
        Func<IScopeBuilder<TEntity>> scopeBuilderProvider, IScopeBuilder<TEntity> currentScopeBuilder)
    {
        return new ScopeTracker<TEntity>(scopeBuilderProvider, currentScopeBuilder);
    }
}
