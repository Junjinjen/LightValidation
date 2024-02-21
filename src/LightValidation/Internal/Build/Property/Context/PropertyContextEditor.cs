using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using System;
using System.Collections.Generic;

namespace LightValidation.Internal.Build.Property.Context;

internal interface IPropertyContextEditor
{
    void SetExecutionModeForAttribute(Type attributeType, ExecutionMode mode);

    void SetDefaultExecutionMode(ExecutionMode mode);

    void SetPropertyName(string propertyName);

    IPropertyBuildContext Edit(IPropertyBuildContext context);
}

internal sealed class PropertyContextEditor : BuilderBase, IPropertyContextEditor
{
    private readonly IPropertyContextFactory _propertyContextFactory;

    private Dictionary<Type, ExecutionMode>? _executionModeByAttribute;
    private ExecutionMode? _defaultExecutionMode;
    private string? _propertyName;

    public PropertyContextEditor(IPropertyContextFactory propertyContextFactory)
    {
        _propertyContextFactory = propertyContextFactory;
    }

    public void SetExecutionModeForAttribute(Type attributeType, ExecutionMode mode)
    {
        attributeType.EnsureAttributeType();
        mode.EnsureDefined();
        EnsureNotBuilt();

        _executionModeByAttribute ??= [];
        _executionModeByAttribute[attributeType] = mode;
    }

    public void SetDefaultExecutionMode(ExecutionMode mode)
    {
        mode.EnsureDefined();
        EnsureNotBuilt();

        _defaultExecutionMode = mode;
    }

    public void SetPropertyName(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        EnsureNotBuilt();

        _propertyName = propertyName;
    }

    public IPropertyBuildContext Edit(IPropertyBuildContext context)
    {
        SetBuilt();

        if (_executionModeByAttribute == null && _defaultExecutionMode == null && _propertyName == null)
        {
            return context;
        }

        UpdateExecutionModeByAttribute(context);

        var parameters = new PropertyContextParameters
        {
            EntityBuildContext = context.EntityBuildContext,
            ExecutionModeByAttribute = _executionModeByAttribute ?? context.ExecutionModeByAttribute,
            DefaultExecutionMode = _defaultExecutionMode ?? context.DefaultExecutionMode,
            PropertySelectorExpression = context.PropertySelectorExpression,
            PropertySelector = context.PropertySelector,
            PropertyName = _propertyName ?? context.PropertyName,
        };

        return _propertyContextFactory.Create(parameters);
    }

    private void UpdateExecutionModeByAttribute(IPropertyBuildContext context)
    {
        if (_executionModeByAttribute == null)
        {
            return;
        }

        foreach (var pair in context.ExecutionModeByAttribute)
        {
            if (!_executionModeByAttribute.ContainsKey(pair.Key))
            {
                _executionModeByAttribute.Add(pair.Key, pair.Value);
            }
        }
    }
}
