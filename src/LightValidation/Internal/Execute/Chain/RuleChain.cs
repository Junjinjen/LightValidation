using LightValidation.Abstractions.Execute;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Chain;

internal sealed class RuleChain<TEntity, TProperty> : RuleChainBase<TEntity, TProperty, TProperty>
{
    public RuleChain(
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? condition,
        IPropertyValidator<TEntity, TProperty>[] propertyValidators,
        int metadataId)
        : base(condition, propertyValidators, metadataId)
    {
        Debug.Assert(
            propertyValidators.Length > 1 || condition != null,
            "Rule chain with a single property validator must have a condition.");
    }

    public override async ValueTask Validate(
        IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode)
    {
        Debug.Assert(ExecutionModes.Contains(currentMode),
            "The current execution mode is not supported by the validator.");

        var condition = await VerifyCondition(context).ConfigureAwait(false);
        if (!condition)
        {
            return;
        }

        foreach (var validator in _propertyValidators)
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

    private ValueTask<bool> VerifyCondition(IPropertyValidationContext<TEntity, TProperty> context)
    {
        if (_condition == null)
        {
            return ValueTask.FromResult(true);
        }

        var value = context.GetValidationMetadata(_metadataId);
        if (value != null)
        {
            var result = ReferenceEquals(value, Constants.TrueObjectValue);

            return ValueTask.FromResult(result);
        }

        return ExecuteCondition(context);
    }

    private async ValueTask<bool> ExecuteCondition(IPropertyValidationContext<TEntity, TProperty> context)
    {
        var result = await _condition!.Invoke(context.ValidationContext, context.PropertyValue).ConfigureAwait(false);
        var metadata = result ? Constants.TrueObjectValue : Constants.FalseObjectValue;

        context.SetValidationMetadata(_metadataId, metadata);

        return result;
    }
}
