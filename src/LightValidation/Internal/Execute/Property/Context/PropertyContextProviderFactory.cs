using System;

namespace LightValidation.Internal.Execute.Property.Context;

internal interface IPropertyContextProviderFactory
{
    IPropertyContextProvider<TEntity, TProperty> Create<TEntity, TProperty>(
        Func<TEntity, TProperty> propertySelector, int metadataId);
}

internal sealed class PropertyContextProviderFactory : IPropertyContextProviderFactory
{
    public IPropertyContextProvider<TEntity, TProperty> Create<TEntity, TProperty>(
        Func<TEntity, TProperty> propertySelector, int metadataId)
    {
        return new PropertyContextProvider<TEntity, TProperty>(
            propertySelector, metadataId, DependencyResolver.PropertyValidationContextFactory);
    }
}
