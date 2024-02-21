using LightValidation.Abstractions.Execute;
using System;

namespace LightValidation.Internal.Execute.Property.Context;

internal interface IPropertyContextProvider<TEntity, TProperty>
{
    IPropertyValidationContext<TEntity, TProperty> Get(IEntityValidationContext<TEntity> context);
}

internal sealed class PropertyContextProvider<TEntity, TProperty> : IPropertyContextProvider<TEntity, TProperty>
{
    private readonly IPropertyContextFactory _propertyContextFactory;

    private readonly Func<TEntity, TProperty> _propertySelector;
    private readonly int _metadataId;

    public PropertyContextProvider(
        Func<TEntity, TProperty> propertySelector, int metadataId, IPropertyContextFactory propertyContextFactory)
    {
        _propertySelector = propertySelector;
        _metadataId = metadataId;

        _propertyContextFactory = propertyContextFactory;
    }

    public IPropertyValidationContext<TEntity, TProperty> Get(IEntityValidationContext<TEntity> context)
    {
        var value = context.GetValidationMetadata(_metadataId);
        if (value == null)
        {
            var propertyValue = _propertySelector.Invoke(context.ValidationContext.Entity);
            value = _propertyContextFactory.Create(context, propertyValue);

            context.SetValidationMetadata(_metadataId, value);
        }

        return (PropertyContext<TEntity, TProperty>)value;
    }
}
