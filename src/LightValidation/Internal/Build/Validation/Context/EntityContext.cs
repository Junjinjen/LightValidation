using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;

namespace LightValidation.Internal.Build.Validation.Context;

internal interface IEntityContextInternal : IEntityBuildContext
{
    int MetadataCount { get; }

    void SetBuilt();
}

internal sealed class EntityContext : BuildContextBase, IEntityContextInternal
{
    private readonly IReadOnlyDictionary<Type, ExecutionMode> _executionModeByAttribute;
    private readonly IReadOnlyDictionary<string, string> _propertyNames;

    private readonly ExecutionMode _defaultExecutionMode;
    private readonly Type _validatorType;

    public EntityContext(
        ExecutionMode defaultExecutionMode,
        Type validatorType,
        IReadOnlyDictionary<Type, ExecutionMode> executionModeByAttribute,
        IReadOnlyDictionary<string, string> propertyNames)
    {
        _defaultExecutionMode = defaultExecutionMode;
        _validatorType = validatorType;

        _executionModeByAttribute = executionModeByAttribute;
        _propertyNames = propertyNames;
    }

    public Type ValidatorType => ReturnWithBuildCheck(_validatorType);

    public IReadOnlyDictionary<string, string> PropertyNames => ReturnWithBuildCheck(_propertyNames);

    public IReadOnlyDictionary<Type, ExecutionMode> ExecutionModeByAttribute =>
        ReturnWithBuildCheck(_executionModeByAttribute);

    public ExecutionMode DefaultExecutionMode => ReturnWithBuildCheck(_defaultExecutionMode);

    public int MetadataCount { get; private set; }

    public bool IsValidationBuilt { get; private set; }

    protected override bool IsBuilt => IsValidationBuilt;

    public int RegisterMetadata()
    {
        EnsureNotBuilt();

        return MetadataCount++;
    }

    public void SetBuilt()
    {
        IsValidationBuilt = true;
    }
}
