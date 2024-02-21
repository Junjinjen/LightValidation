using LightValidation.Abstractions.Execute;

namespace LightValidation.Internal.Execute.Nested;

internal interface INestedValidationExecutorFactory
{
    IPropertyValidator<TEntity, TProperty> Create<TEntity, TProperty>(
        in NestedValidationExecutorParameters<TEntity, TProperty> parameters);
}

internal sealed class NestedValidationExecutorFactory : INestedValidationExecutorFactory
{
    public IPropertyValidator<TEntity, TProperty> Create<TEntity, TProperty>(
        in NestedValidationExecutorParameters<TEntity, TProperty> parameters)
    {
        return new NestedValidationExecutor<TEntity, TProperty>(
            parameters.ValidatorProvider,
            parameters.Condition,
            parameters.PropertyName,
            parameters.MetadataId,
            DependencyResolver.ValidationExecutorCache,
            DependencyResolver.NestedContextFactory);
    }
}
