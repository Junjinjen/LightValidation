using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;

namespace LightValidation.Internal.Build.Validation.Context;

internal interface IEntityContextInternal : IEntityBuildContext
{
    int MetadataCount { get; }
}

internal sealed class EntityContext : IEntityContextInternal
{
    public required Type ValidatorType { get; init; }

    public required IReadOnlyDictionary<string, string> PropertyNames { get; init; }

    public required IReadOnlyDictionary<Type, ExecutionMode> ExecutionModeByAttribute { get; init; }

    public required ExecutionMode DefaultExecutionMode { get; init; }

    public int MetadataCount { get; private set; }

    public int RegisterMetadata()
    {
        return MetadataCount++;
    }
}
