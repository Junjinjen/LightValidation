using LightValidation.Abstractions.Build;

namespace LightValidation.Internal.Build.Property;

internal interface IPropertyValidationBuilderFactory
{
    IEntityValidatorBuilder<TEntity> Create<TEntity, TProperty>(
        in PropertyValidationBuilderParameters<TEntity, TProperty> parameters);
}

internal sealed class PropertyValidationBuilderFactory : IPropertyValidationBuilderFactory
{
    public IEntityValidatorBuilder<TEntity> Create<TEntity, TProperty>(
        in PropertyValidationBuilderParameters<TEntity, TProperty> parameters)
    {
        return new PropertyValidationBuilder<TEntity, TProperty>(
            parameters.PropertySelectorExpression,
            parameters.PropertyPath,
            parameters.PropertyValidatorBuilder,
            DependencyResolver.PropertyValidationExecutorFactory,
            parameters.PropertyContextProviderCache,
            parameters.PropertyContextCache);
    }
}
