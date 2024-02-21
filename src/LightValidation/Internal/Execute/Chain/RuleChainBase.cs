using LightValidation.Abstractions.Execute;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Chain;

internal abstract class RuleChainBase<TEntity, TElement, TProperty> : IPropertyValidator<TEntity, TProperty>
{
    protected readonly Func<ValidationContext<TEntity>, TElement, ValueTask<bool>>? _condition;
    protected readonly IPropertyValidator<TEntity, TElement>[] _propertyValidators;
    protected readonly int _metadataId;

    protected RuleChainBase(
        Func<ValidationContext<TEntity>, TElement, ValueTask<bool>>? condition,
        IPropertyValidator<TEntity, TElement>[] propertyValidators,
        int metadataId)
    {
        Debug.Assert(propertyValidators.Length != 0, "Rule chain must have property validators.");

        ExecutionModes = new(propertyValidators.SelectMany(x => x.ExecutionModes));

        _condition = condition;
        _propertyValidators = propertyValidators;
        _metadataId = metadataId;
    }

    public ExecutionModeCollection ExecutionModes { get; }

    public abstract ValueTask Validate(
        IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode);

    protected static bool CanExecute(IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode mode)
    {
        if (mode == ExecutionMode.Always)
        {
            return true;
        }

        if (mode == ExecutionMode.OnValidProperty)
        {
            return context.IsPropertyValid;
        }

        Debug.Assert(mode == ExecutionMode.OnValidEntity, $"Execution mode value \"{mode}\" is invalid.");

        return context.IsEntityValid;
    }
}
