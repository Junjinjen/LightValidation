using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightValidation.Internal.Build.Rule;

internal interface IExecutionModeProvider
{
    void SetExecutionMode(ExecutionMode mode);

    ExecutionMode GetExecutionMode(
        IReadOnlyDictionary<Type, ExecutionMode> executionModeByAttribute,
        ExecutionMode defaultExecutionMode,
        Type ruleType);
}

internal sealed class ExecutionModeProvider : IExecutionModeProvider
{
    private ExecutionMode? _mode;

    public void SetExecutionMode(ExecutionMode mode)
    {
        mode.EnsureDefined();

        _mode = mode;
    }

    public ExecutionMode GetExecutionMode(
        IReadOnlyDictionary<Type, ExecutionMode> executionModeByAttribute,
        ExecutionMode defaultExecutionMode,
        Type ruleType)
    {
        if (_mode != null)
        {
            return _mode.Value;
        }

        var attributeType = executionModeByAttribute.Keys.SingleOrDefault(
            x => Attribute.GetCustomAttribute(ruleType, x) != null);

        return attributeType != null ? executionModeByAttribute[attributeType] : defaultExecutionMode;
    }
}
