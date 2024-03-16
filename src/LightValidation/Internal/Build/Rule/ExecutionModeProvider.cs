using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Extensions;
using LightValidation.Internal.Build.Extensions;
using System;

namespace LightValidation.Internal.Build.Rule;

internal interface IExecutionModeProvider
{
    void SetExecutionMode(ExecutionMode mode);

    ExecutionMode GetExecutionMode(IExecutionModeContext context, Type ruleType);
}

internal sealed class ExecutionModeProvider : IExecutionModeProvider
{
    private ExecutionMode? _mode;

    public void SetExecutionMode(ExecutionMode mode)
    {
        mode.EnsureDefined();

        _mode = mode;
    }

    public ExecutionMode GetExecutionMode(IExecutionModeContext context, Type ruleType)
    {
        return _mode != null ? _mode.Value : context.GetExecutionMode(ruleType);
    }
}
