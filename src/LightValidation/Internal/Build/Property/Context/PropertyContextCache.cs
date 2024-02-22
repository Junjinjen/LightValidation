using LightValidation.Abstractions.Build;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal.Build.Property.Context;

internal interface IPropertyContextCache<TEntity>
{
    IPropertyBuildContext Get<TProperty>(
        string propertyPath,
        Expression<Func<TEntity, TProperty>> propertySelectorExpression,
        IEntityBuildContext context);
}

internal sealed class PropertyContextCache<TEntity> : IPropertyContextCache<TEntity>
{
    private readonly IPropertySelectorCache<TEntity> _propertySelectorCache;
    private readonly IPropertyContextFactory _propertyContextFactory;

    private readonly Dictionary<string, IPropertyBuildContext> _cache = [];

    public PropertyContextCache(
        IPropertySelectorCache<TEntity> propertySelectorCache, IPropertyContextFactory propertyContextFactory)
    {
        _propertySelectorCache = propertySelectorCache;
        _propertyContextFactory = propertyContextFactory;
    }

    public IPropertyBuildContext Get<TProperty>(
        string propertyPath,
        Expression<Func<TEntity, TProperty>> propertySelectorExpression,
        IEntityBuildContext context)
    {
        if (!_cache.TryGetValue(propertyPath, out var value))
        {
            var propertySelector = _propertySelectorCache.Get(propertyPath, propertySelectorExpression);
            var propertyName = GetPropertyName(propertyPath, context);

            var parameters = new PropertyContextParameters
            {
                PropertySelectorExpression = propertySelectorExpression,
                DefaultExecutionMode = context.DefaultExecutionMode,
                PropertySelector = propertySelector,
                PropertyName = propertyName,
                ExecutionModeByAttribute = context.ExecutionModeByAttribute,
                EntityBuildContext = context,
            };

            value = _propertyContextFactory.Create(parameters);

            _cache.Add(propertyPath, value);
        }

        return value;
    }

    private static string GetPropertyName(string propertyPath, IEntityBuildContext context)
    {
        return context.PropertyNames.GetValueOrDefault(propertyPath, propertyPath);
    }
}
