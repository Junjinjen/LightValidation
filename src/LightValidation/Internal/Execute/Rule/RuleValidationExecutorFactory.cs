using LightValidation.Abstractions.Execute;

namespace LightValidation.Internal.Execute.Rule;

internal interface IRuleValidationExecutorFactory
{
    IPropertyValidator<TEntity, TProperty> Create<TEntity, TProperty>(
        in RuleValidationExecutorParameters<TEntity, TProperty> parameters);
}

internal sealed class RuleValidationExecutorFactory : IRuleValidationExecutorFactory
{
    public IPropertyValidator<TEntity, TProperty> Create<TEntity, TProperty>(
        in RuleValidationExecutorParameters<TEntity, TProperty> parameters)
    {
        return new RuleValidationExecutor<TEntity, TProperty>(
            parameters.ExecutionMode,
            parameters.Condition,
            parameters.FailureGenerator,
            parameters.DependentScope,
            parameters.Rule);
    }
}
