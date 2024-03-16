using LightValidation.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LightValidation;

public sealed class ValidationContext<TEntity> : IDependencyResolver
{
    private readonly IDependencyResolver? _dependencyResolver;

    private readonly Dictionary<Type, object?>? _services;

    public ValidationContext(Dictionary<Type, object?>? services, IDependencyResolver? dependencyResolver)
    {
        _services = services;

        _dependencyResolver = dependencyResolver;
    }

    public required TEntity Entity { get; init; }

    public required ValidationCache Cache { get; init; }

    public required RuleSetCollection RuleSets { get; init; }

    public required CancellationToken CancellationToken { get; init; }

    public TService GetService<TService>()
    {
        if (_services?.TryGetValue(typeof(TService), out var service) == true)
        {
            return (TService)service!;
        }

        if (_dependencyResolver != null)
        {
            return _dependencyResolver.GetService<TService>();
        }

        throw new InvalidOperationException($"No service for type '{typeof(TService)}' has been registered.");
    }
}
