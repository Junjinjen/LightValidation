using LightValidation.Abstractions.Execute;
using LightValidation.Result;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Validation;

internal interface IValidationExecutor<TEntity> : IEntityValidator<TEntity>
{
    int MetadataCount { get; }
}

internal sealed class ValidationExecutor<TEntity> : IValidationExecutor<TEntity>
{
    private readonly IEntityValidator<TEntity>[] _entityValidators;
    private readonly NullEntityFailure? _nullEntityFailure;

    public ValidationExecutor(IEntityValidator<TEntity>[] entityValidators, NullEntityFailure? nullEntityFailure)
    {
        ExecutionModes = new(entityValidators.SelectMany(x => x.ExecutionModes));

        _entityValidators = entityValidators;
        _nullEntityFailure = nullEntityFailure;
    }

    public required int MetadataCount { get; init; }

    public ExecutionModeCollection ExecutionModes { get; }

    public ValueTask Validate(IEntityValidationContext<TEntity> context, ExecutionMode currentMode)
    {
        Debug.Assert(ExecutionModes.Contains(currentMode),
            "The current execution mode is not supported by the validator.");

        context.ValidationContext.CancellationToken.ThrowIfCancellationRequested();

        if (context.ValidationContext.Entity == null)
        {
            return HandleNullEntity(context);
        }

        return ValidateInternal(context, currentMode);
    }

    private static bool CanExecute(IEntityValidationContext<TEntity> context, ExecutionMode currentMode)
    {
        return currentMode != ExecutionMode.OnValidEntity || context.IsEntityValid;
    }

    private ValueTask HandleNullEntity(IEntityValidationContext<TEntity> context)
    {
        if (_nullEntityFailure != null && context.IsEntityValid)
        {
            context.AddRuleFailure(_nullEntityFailure);
        }

        return ValueTask.CompletedTask;
    }

    private async ValueTask ValidateInternal(IEntityValidationContext<TEntity> context, ExecutionMode currentMode)
    {
        foreach (var validator in _entityValidators)
        {
            if (!CanExecute(context, currentMode))
            {
                return;
            }

            if (!validator.ExecutionModes.Contains(currentMode))
            {
                continue;
            }

            await validator.Validate(context, currentMode).ConfigureAwait(false);
        }
    }
}
