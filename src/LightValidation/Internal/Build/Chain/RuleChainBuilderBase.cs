using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Property.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Chain;

internal abstract class RuleChainBuilderBase<TEntity, TProperty>
    : BuilderBase, IRuleChainConfiguration<TEntity, TProperty>
{
    private readonly IPropertyConditionBuilder<TEntity, TProperty> _propertyConditionBuilder;
    private readonly IPropertyContextEditor _propertyContextEditor;
    private readonly IScopeTracker _scopeTracker;

    private readonly List<IPropertyValidatorBuilder<TEntity, TProperty>> _propertyValidatorBuilders = [];

    protected RuleChainBuilderBase(
        IValidationBuilder<TEntity> validationBuilder,
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        IPropertyContextEditor propertyContextEditor)
    {
        _scopeTracker = validationBuilder.RememberCurrentScope();

        ValidationBuilder = validationBuilder;

        _propertyConditionBuilder = propertyConditionBuilder;
        _propertyContextEditor = propertyContextEditor;
    }

    public IValidationBuilder<TEntity> ValidationBuilder { get; }

    public void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        EnsureNotBuilt();
        _scopeTracker.EnsureScopeUnchanged();

        _propertyConditionBuilder.AddCondition(condition);
    }

    public void SetExecutionModeForAttribute(Type attributeType, ExecutionMode mode)
    {
        EnsureNotBuilt();
        _scopeTracker.EnsureScopeUnchanged();

        _propertyContextEditor.SetExecutionModeForAttribute(attributeType, mode);
    }

    public void SetDefaultExecutionMode(ExecutionMode mode)
    {
        EnsureNotBuilt();
        _scopeTracker.EnsureScopeUnchanged();

        _propertyContextEditor.SetDefaultExecutionMode(mode);
    }

    public void SetPropertyName(string propertyName)
    {
        EnsureNotBuilt();
        _scopeTracker.EnsureScopeUnchanged();

        _propertyContextEditor.SetPropertyName(propertyName);
    }

    public void AddPropertyValidator(IPropertyValidatorBuilder<TEntity, TProperty> validatorBuilder)
    {
        ArgumentNullException.ThrowIfNull(validatorBuilder);
        EnsureNotBuilt();
        _scopeTracker.EnsureScopeUnchanged();

        _propertyValidatorBuilders.Add(validatorBuilder);
    }

    protected IPropertyValidator<TEntity, TProperty>[] BuildPropertyValidators(IPropertyBuildContext context)
    {
        if (_propertyValidatorBuilders.Count == 0)
        {
            return Array.Empty<IPropertyValidator<TEntity, TProperty>>();
        }

        var modifiedContext = _propertyContextEditor.Edit(context);

        return _propertyValidatorBuilders.Select(x => x.Build(modifiedContext)).WhereNotNull().ToArray();
    }

    protected Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? BuildCondition()
    {
        return _propertyConditionBuilder.Build();
    }
}
