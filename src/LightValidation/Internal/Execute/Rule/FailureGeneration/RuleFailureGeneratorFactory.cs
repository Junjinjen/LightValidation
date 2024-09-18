namespace LightValidation.Internal.Execute.Rule.FailureGeneration;

internal interface IRuleFailureGeneratorFactory
{
    IRuleFailureGenerator<TEntity, TProperty> Create<TEntity, TProperty>(
        in RuleFailureGeneratorParameters<TEntity, TProperty> parameters);
}

internal sealed class RuleFailureGeneratorFactory : IRuleFailureGeneratorFactory
{
    public IRuleFailureGenerator<TEntity, TProperty> Create<TEntity, TProperty>(
        in RuleFailureGeneratorParameters<TEntity, TProperty> parameters)
    {
        return new RuleFailureGenerator<TEntity, TProperty>(
            parameters.AppendCollectionIndexToPropertyName,
            parameters.PropertyName,
            parameters.ErrorCode,
            parameters.MetadataGenerator,
            parameters.DescriptionGenerator);
    }
}
