using LightValidation.Abstractions.Build;
using System;

namespace LightValidation.Internal.Build.Scope.Track;

internal sealed class ScopeTracker<TEntity> : IScopeTracker
{
    private const string DefaultErrorMessage = "Scope change detected. " +
        "Validation configuration must be performed in the same scope in which it was defined.";

    private readonly IScopeBuilder<TEntity> _currentScopeBuilder;

    private readonly Func<IScopeBuilder<TEntity>> _scopeBuilderProvider;

    public ScopeTracker(Func<IScopeBuilder<TEntity>> scopeBuilderProvider, IScopeBuilder<TEntity> currentScopeBuilder)
    {
        _scopeBuilderProvider = scopeBuilderProvider;

        _currentScopeBuilder = currentScopeBuilder;
    }

    public void EnsureScopeUnchanged(string? errorMessage = null)
    {
        var scopeBuilder = _scopeBuilderProvider.Invoke();
        var result = ReferenceEquals(scopeBuilder, _currentScopeBuilder);
        if (!result)
        {
            var message = string.IsNullOrEmpty(errorMessage) ? DefaultErrorMessage : errorMessage;

            throw new InvalidOperationException(message);
        }
    }
}
