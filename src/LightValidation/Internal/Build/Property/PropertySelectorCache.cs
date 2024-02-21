using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal.Build.Property;

internal interface IPropertySelectorCache<TEntity>
{
    Func<TEntity, TProperty> Get<TProperty>(
        string propertyPath, Expression<Func<TEntity, TProperty>> propertySelectorExpression);
}

internal sealed class PropertySelectorCache<TEntity> : IPropertySelectorCache<TEntity>
{
    private readonly Dictionary<string, object> _cache = [];

    public Func<TEntity, TProperty> Get<TProperty>(
        string propertyPath, Expression<Func<TEntity, TProperty>> propertySelectorExpression)
    {
        if (!_cache.TryGetValue(propertyPath, out var value))
        {
            value = propertySelectorExpression.Compile();

            _cache.Add(propertyPath, value);
        }

        return (Func<TEntity, TProperty>)value;
    }
}
