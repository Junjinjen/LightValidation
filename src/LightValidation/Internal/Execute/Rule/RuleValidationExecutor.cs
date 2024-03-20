using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Rule.FailureGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Rule;

internal sealed class RuleValidationExecutor<TEntity, TProperty> : IPropertyValidator<TEntity, TProperty>
{
    private readonly IRuleFailureGenerator<TEntity, TProperty> _failureGenerator;
    private readonly IEntityValidator<TEntity>? _dependentScope;
    private readonly IPropertyRule<TEntity, TProperty> _rule;

    private readonly Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? _condition;

    public RuleValidationExecutor(
        ExecutionMode mode,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? condition,
        IRuleFailureGenerator<TEntity, TProperty> failureGenerator,
        IEntityValidator<TEntity>? dependentScope,
        IPropertyRule<TEntity, TProperty> rule)
    {
        ExecutionModes = CalculateExecutionModes(mode, dependentScope);

        _condition = condition;

        _failureGenerator = failureGenerator;
        _dependentScope = dependentScope;
        _rule = rule;
    }

    public ExecutionModeCollection ExecutionModes { get; }

    public ValueTask Validate(IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode)
    {
        Debug.Assert(ExecutionModes.Contains(currentMode),
            "The current execution mode is not supported by the validator.");

        return _dependentScope == null
            ? ValidateWithoutDependent(context)
            : ValidateWithDependent(context, currentMode);
    }

    private static ExecutionModeCollection CalculateExecutionModes(
        ExecutionMode mode, IEntityValidator<TEntity>? dependentScope)
    {
        if (dependentScope == null)
        {
            return new([mode]);
        }

        var modes = new List<ExecutionMode> { mode };
        foreach (var dependentMode in dependentScope.ExecutionModes)
        {
            if (dependentMode > mode)
            {
                modes.Add(dependentMode);
            }
        }

        return new(modes);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask ValidateWithDependent(
        IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode)
    {
        if (!IsFirstExecution(currentMode))
        {
            await ExecuteDependentScope(context, currentMode).ConfigureAwait(false);

            return;
        }

        var condition = await VerifyCondition(context).ConfigureAwait(false);
        if (!condition)
        {
            return;
        }

        var result = await _rule.Validate(context.ValidationContext, context.PropertyValue).ConfigureAwait(false);
        if (!result)
        {
            var failure = _failureGenerator.Generate(context);
            context.AddRuleFailure(failure);

            return;
        }

        if (!context.CanExecuteDependentRules)
        {
            return;
        }

        await PreExecuteDependentScope(context, currentMode).ConfigureAwait(false);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask ValidateWithoutDependent(IPropertyValidationContext<TEntity, TProperty> context)
    {
        var condition = await VerifyCondition(context).ConfigureAwait(false);
        if (!condition)
        {
            return;
        }

        var result = await _rule.Validate(context.ValidationContext, context.PropertyValue).ConfigureAwait(false);
        if (!result)
        {
            var failure = _failureGenerator.Generate(context);
            context.AddRuleFailure(failure);
        }
    }

    private ValueTask<bool> VerifyCondition(IPropertyValidationContext<TEntity, TProperty> context)
    {
        if (_condition == null)
        {
            return ValueTask.FromResult(true);
        }

        return _condition.Invoke(context.ValidationContext, context.PropertyValue);
    }

    private bool IsFirstExecution(ExecutionMode currentMode)
    {
        if (ExecutionModes.Contains(ExecutionMode.Always))
        {
            return currentMode == ExecutionMode.Always;
        }

        if (ExecutionModes.Contains(ExecutionMode.OnValidProperty))
        {
            return currentMode == ExecutionMode.OnValidProperty;
        }

        Debug.Assert(ExecutionModes.Contains(ExecutionMode.OnValidEntity), "Invalid execution mode collection state.");

        return currentMode == ExecutionMode.OnValidEntity;
    }

    private ValueTask ExecuteDependentScope(
        IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode)
    {
        if (!context.CanExecuteDependentRules)
        {
            return ValueTask.CompletedTask;
        }

        return _dependentScope!.Validate(context.EntityValidationContext, currentMode);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask PreExecuteDependentScope(
        IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode)
    {
        foreach (var mode in _dependentScope!.ExecutionModes)
        {
            if (mode > currentMode)
            {
                return;
            }

            await _dependentScope!.Validate(context.EntityValidationContext, mode).ConfigureAwait(false);
        }
    }
}
