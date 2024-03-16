using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;

namespace LightValidation.Abstractions.Build;

public interface IExecutionModeContext
{
    IReadOnlyDictionary<Type, ExecutionMode> ExecutionModeByAttribute { get; }

    ExecutionMode DefaultExecutionMode { get; }
}
