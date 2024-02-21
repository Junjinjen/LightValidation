using LightValidation.Abstractions.Build;
using LightValidation.Internal.Execute.Property.Context;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal.Build.Property.Context;

internal interface IPropertyContextProviderCache<TEntity>
{
    IPropertyContextProvider<TEntity, TProperty> Get<TProperty>(
        string propertyPath,
        Expression<Func<TEntity, TProperty>> propertySelectorExpression,
        IEntityBuildContext context);
}

internal sealed class PropertyContextProviderCache<TEntity> : IPropertyContextProviderCache<TEntity>
{
    private readonly IPropertyContextProviderFactory _propertyContextProviderFactory;
    private readonly IPropertySelectorCache<TEntity> _propertySelectorCache;

    private readonly Dictionary<string, object> _cache = [];

    public PropertyContextProviderCache(
        IPropertyContextProviderFactory propertyContextProviderFactory,
        IPropertySelectorCache<TEntity> propertySelectorCache)
    {
        _propertyContextProviderFactory = propertyContextProviderFactory;
        _propertySelectorCache = propertySelectorCache;
    }

    public IPropertyContextProvider<TEntity, TProperty> Get<TProperty>(
        string propertyPath,
        Expression<Func<TEntity, TProperty>> propertySelectorExpression,
        IEntityBuildContext context)
    {
        if (!_cache.TryGetValue(propertyPath, out var value))
        {
            var metadataId = context.RegisterMetadata();
            var propertySelector = _propertySelectorCache.Get(propertyPath, propertySelectorExpression);
            value = _propertyContextProviderFactory.Create(propertySelector, metadataId);

            _cache.Add(propertyPath, value);
        }

        return (IPropertyContextProvider<TEntity, TProperty>)value;
    }
}
