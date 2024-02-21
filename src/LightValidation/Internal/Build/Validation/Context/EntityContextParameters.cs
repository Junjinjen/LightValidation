using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;

namespace LightValidation.Internal.Build.Validation.Context;

internal readonly ref struct EntityContextParameters
{
    public required Type ValidatorType { get; init; }

    public required IReadOnlyDictionary<string, string> PropertyNames { get; init; }

    public required IReadOnlyDictionary<Type, ExecutionMode> ExecutionModeByAttribute { get; init; }

    public required ExecutionMode DefaultExecutionMode { get; init; }
}
