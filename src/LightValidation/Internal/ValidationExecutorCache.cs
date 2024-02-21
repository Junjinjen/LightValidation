using LightValidation.Internal.Execute.Validation;
using System;
using System.Collections.Concurrent;

namespace LightValidation.Internal;

internal interface IValidationExecutorCache
{
    IValidationExecutor<TEntity> Get<TEntity>(IValidatorInternal<TEntity> validator);
}

internal sealed class ValidationExecutorCache : IValidationExecutorCache
{
    private readonly ConcurrentDictionary<Type, Lazy<object>> _cache = new();

    public IValidationExecutor<TEntity> Get<TEntity>(IValidatorInternal<TEntity> validator)
    {
        var validatorType = validator.GetType();
        if (!_cache.TryGetValue(validatorType, out var lazy))
        {
            lazy = _cache.GetOrAdd(validatorType, new Lazy<object>(validator.CreateValidationExecutor));
        }

        return (IValidationExecutor<TEntity>)lazy.Value;
    }
}
