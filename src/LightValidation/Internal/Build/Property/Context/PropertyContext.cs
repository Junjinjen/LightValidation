using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal.Build.Property.Context;

internal sealed class PropertyContext : BuildContextBase, IPropertyBuildContext
{
    private readonly IReadOnlyDictionary<Type, ExecutionMode> _executionModeByAttribute;
    private readonly IEntityBuildContext _entityBuildContext;

    private readonly LambdaExpression _propertySelectorExpression;
    private readonly ExecutionMode _defaultExecutionMode;
    private readonly Delegate _propertySelector;
    private readonly string _propertyName;

    public PropertyContext(
        LambdaExpression propertySelectorExpression,
        ExecutionMode defaultExecutionMode,
        Delegate propertySelector,
        string propertyName,
        IReadOnlyDictionary<Type, ExecutionMode> executionModeByAttribute,
        IEntityBuildContext entityBuildContext)
    {
        _propertySelectorExpression = propertySelectorExpression;
        _defaultExecutionMode = defaultExecutionMode;
        _propertySelector = propertySelector;
        _propertyName = propertyName;

        _executionModeByAttribute = executionModeByAttribute;
        _entityBuildContext = entityBuildContext;
    }

    public IEntityBuildContext EntityBuildContext => ReturnWithBuildCheck(_entityBuildContext);

    public Type ValidatorType => ReturnWithBuildCheck(EntityBuildContext.ValidatorType);

    public IReadOnlyDictionary<Type, ExecutionMode> ExecutionModeByAttribute =>
        ReturnWithBuildCheck(_executionModeByAttribute);

    public ExecutionMode DefaultExecutionMode => ReturnWithBuildCheck(_defaultExecutionMode);

    public LambdaExpression PropertySelectorExpression => ReturnWithBuildCheck(_propertySelectorExpression);

    public Delegate PropertySelector => ReturnWithBuildCheck(_propertySelector);

    public string PropertyName => ReturnWithBuildCheck(_propertyName);

    protected override bool IsBuilt => _entityBuildContext.IsValidationBuilt;

    public int RegisterMetadata()
    {
        EnsureNotBuilt();

        return EntityBuildContext.RegisterMetadata();
    }
}
