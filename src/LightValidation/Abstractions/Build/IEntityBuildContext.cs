using System;
using System.Collections.Generic;

namespace LightValidation.Abstractions.Build;

public interface IEntityBuildContext : IExecutionModeContext
{
    Type ValidatorType { get; }

    IReadOnlyDictionary<string, string> PropertyNames { get; }

    bool IsValidationBuilt { get; }

    int RegisterMetadata();
}
