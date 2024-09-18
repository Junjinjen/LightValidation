using LightValidation.Abstractions.Execute;

namespace LightValidation.Internal.Execute.Property.Context;

internal interface IPropertyContextFactory
{
    IPropertyValidationContext<TEntity, TProperty> Create<TEntity, TProperty>(
        IEntityValidationContext<TEntity> context, TProperty propertyValue);
}

internal sealed class PropertyContextFactory : IPropertyContextFactory
{
    public IPropertyValidationContext<TEntity, TProperty> Create<TEntity, TProperty>(
        IEntityValidationContext<TEntity> context, TProperty propertyValue)
    {
        return new PropertyContext<TEntity, TProperty>
        {
            EntityContext = context,
            PropertyValue = propertyValue,
        };
    }
}
