using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Property.Context;

namespace LightValidation.Internal.Execute.Property;

internal interface IPropertyValidationExecutorFactory
{
    IEntityValidator<TEntity> Create<TEntity, TProperty>(
        IPropertyContextProvider<TEntity, TProperty> propertyContextProvider,
        IPropertyValidator<TEntity, TProperty> propertyValidator);
}

internal sealed class PropertyValidationExecutorFactory : IPropertyValidationExecutorFactory
{
    public IEntityValidator<TEntity> Create<TEntity, TProperty>(
        IPropertyContextProvider<TEntity, TProperty> propertyContextProvider,
        IPropertyValidator<TEntity, TProperty> propertyValidator)
    {
        return new PropertyValidationExecutor<TEntity, TProperty>(propertyContextProvider, propertyValidator);
    }
}
