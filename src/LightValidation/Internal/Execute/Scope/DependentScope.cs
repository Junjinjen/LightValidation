using LightValidation.Abstractions.Execute;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Scope;

internal sealed class DependentScope<TEntity> : IEntityValidator<TEntity>
{
    private readonly IEntityValidator<TEntity>[] _entityValidators;

    public DependentScope(IEntityValidator<TEntity>[] entityValidators)
    {
        Debug.Assert(entityValidators.Length > 1, "Dependent scope must have more than one entity validator.");

        ExecutionModes = new(entityValidators.SelectMany(x => x.ExecutionModes));

        _entityValidators = entityValidators;
    }

    public ExecutionModeCollection ExecutionModes { get; }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Validate(IEntityValidationContext<TEntity> context, ExecutionMode currentMode)
    {
        Debug.Assert(ExecutionModes.Contains(currentMode),
            "The current execution mode is not supported by the validator.");

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
}
