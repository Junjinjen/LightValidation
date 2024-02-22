using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;

namespace LightValidation.Abstractions.Build;

public interface IEntityBuildContext
{
    Type ValidatorType { get; }

    IReadOnlyDictionary<string, string> PropertyNames { get; }

    IReadOnlyDictionary<Type, ExecutionMode> ExecutionModeByAttribute { get; }

    ExecutionMode DefaultExecutionMode { get; }

    bool IsValidationBuilt { get; }

    int RegisterMetadata();
}
