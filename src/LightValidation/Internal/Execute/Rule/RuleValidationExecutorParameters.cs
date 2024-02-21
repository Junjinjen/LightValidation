using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Rule.FailureGeneration;
using System;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Rule;

internal readonly ref struct RuleValidationExecutorParameters<TEntity, TProperty>
{
    public required ExecutionMode ExecutionMode { get; init; }

    public required Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? Condition { get; init; }

    public required IRuleFailureGenerator<TEntity, TProperty> FailureGenerator { get; init; }

    public required IEntityValidator<TEntity>? DependentScope { get; init; }

    public required IPropertyRule<TEntity, TProperty> Rule { get; init; }
}
