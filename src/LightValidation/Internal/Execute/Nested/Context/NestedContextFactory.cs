namespace LightValidation.Internal.Execute.Nested.Context;

internal interface INestedContextFactory
{
    INestedContext<TProperty> Create<TEntity, TProperty>(in NestedContextParameters<TEntity, TProperty> parameters);
}

internal sealed class NestedContextFactory : INestedContextFactory
{
    public INestedContext<TProperty> Create<TEntity, TProperty>(
        in NestedContextParameters<TEntity, TProperty> parameters)
    {
        return new NestedContext<TEntity, TProperty>(
            parameters.MetadataCount, parameters.PropertyName, parameters.PropertyContext)
        {
            ValidationContext = parameters.ValidationContext,
            ValidationExecutor = parameters.ValidationExecutor,
        };
    }
}
