using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Property.Context;
using LightValidation.Internal.Execute.Property;
using System;
using System.Linq.Expressions;

namespace LightValidation.Internal.Build.Property;

internal sealed class PropertyValidationBuilder<TEntity, TProperty> : BuilderBase, IEntityValidatorBuilder<TEntity>
{
    private readonly IPropertyValidatorBuilder<TEntity, TProperty> _propertyValidatorBuilder;
    private readonly IPropertyValidationExecutorFactory _propertyValidationExecutorFactory;
    private readonly IPropertyContextProviderCache<TEntity> _propertyContextProviderCache;
    private readonly IPropertyContextCache<TEntity> _propertyContextCache;

    private readonly Expression<Func<TEntity, TProperty>> _propertySelectorExpression;
    private readonly string _propertyPath;

    public PropertyValidationBuilder(
        Expression<Func<TEntity, TProperty>> propertySelectorExpression,
        string propertyPath,
        IPropertyValidatorBuilder<TEntity, TProperty> propertyValidatorBuilder,
        IPropertyValidationExecutorFactory propertyValidationExecutorFactory,
        IPropertyContextProviderCache<TEntity> propertyContextProviderCache,
        IPropertyContextCache<TEntity> propertyContextCache)
    {
        _propertySelectorExpression = propertySelectorExpression;
        _propertyPath = propertyPath;

        _propertyValidatorBuilder = propertyValidatorBuilder;
        _propertyValidationExecutorFactory = propertyValidationExecutorFactory;
        _propertyContextProviderCache = propertyContextProviderCache;
        _propertyContextCache = propertyContextCache;
    }

    public IEntityValidator<TEntity>? Build(IEntityBuildContext context)
    {
        SetBuilt();

        var propertyContext = _propertyContextCache.Get(_propertyPath, _propertySelectorExpression, context);
        var propertyValidator = _propertyValidatorBuilder.Build(propertyContext);
        if (propertyValidator == null)
        {
            return null;
        }

        var propertyContextProvider = _propertyContextProviderCache.Get(
            _propertyPath, _propertySelectorExpression, context);

        return _propertyValidationExecutorFactory.Create(propertyContextProvider, propertyValidator);
    }
}
