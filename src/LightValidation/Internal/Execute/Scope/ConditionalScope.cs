using LightValidation.Abstractions.Execute;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Scope;

internal sealed class ConditionalScope<TEntity> : IEntityValidator<TEntity>
{
    private readonly Func<ValidationContext<TEntity>, ValueTask<bool>> _condition;
    private readonly IEntityValidator<TEntity>[] _entityValidators;
    private readonly int _metadataId;

    public ConditionalScope(
        Func<ValidationContext<TEntity>, ValueTask<bool>> condition,
        IEntityValidator<TEntity>[] entityValidators,
        int metadataId)
    {
        Debug.Assert(entityValidators.Length != 0, "Conditional scope must have entity validators.");

        ExecutionModes = new(entityValidators.SelectMany(x => x.ExecutionModes));

        _condition = condition;
        _entityValidators = entityValidators;
        _metadataId = metadataId;
    }

    public ExecutionModeCollection ExecutionModes { get; }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Validate(IEntityValidationContext<TEntity> context, ExecutionMode currentMode)
    {
        Debug.Assert(ExecutionModes.Contains(currentMode),
            "The current execution mode is not supported by the validator.");

        var condition = await VerifyCondition(context).ConfigureAwait(false);
        if (!condition)
        {
            return;
        }

        foreach (var validator in _entityValidators)
        {
            if (!validator.ExecutionModes.Contains(currentMode))
            {
                continue;
            }

            await validator.Validate(context, currentMode).ConfigureAwait(false);
            if (!CanExecute(context, currentMode))
            {
                return;
            }
        }
    }

    private static bool CanExecute(IEntityValidationContext<TEntity> context, ExecutionMode currentMode)
    {
        return currentMode != ExecutionMode.OnValidEntity || context.IsEntityValid;
    }

    private ValueTask<bool> VerifyCondition(IEntityValidationContext<TEntity> context)
    {
        var value = context.GetValidationMetadata(_metadataId);
        if (value != null)
        {
            var result = ReferenceEquals(value, Constants.TrueObjectValue);

            return ValueTask.FromResult(result);
        }

        return ExecuteCondition(context);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    private async ValueTask<bool> ExecuteCondition(IEntityValidationContext<TEntity> context)
    {
        var result = await _condition.Invoke(context.ValidationContext).ConfigureAwait(false);
        var metadata = result ? Constants.TrueObjectValue : Constants.FalseObjectValue;

        context.SetValidationMetadata(_metadataId, metadata);

        return result;
    }
}
