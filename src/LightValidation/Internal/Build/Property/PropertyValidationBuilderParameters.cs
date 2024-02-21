using LightValidation.Abstractions.Build;
using LightValidation.Internal.Build.Property.Context;
using System;
using System.Linq.Expressions;

namespace LightValidation.Internal.Build.Property;

internal readonly ref struct PropertyValidationBuilderParameters<TEntity, TProperty>
{
    public required Expression<Func<TEntity, TProperty>> PropertySelectorExpression { get; init; }

    public required string PropertyPath { get; init; }

    public required IPropertyValidatorBuilder<TEntity, TProperty> PropertyValidatorBuilder { get; init; }

    public required IPropertyContextProviderCache<TEntity> PropertyContextProviderCache { get; init; }

    public required IPropertyContextCache<TEntity> PropertyContextCache { get; init; }
}
