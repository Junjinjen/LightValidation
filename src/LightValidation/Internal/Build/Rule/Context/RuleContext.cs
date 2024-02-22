using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Rule.FailureGeneration;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Rule.Context;

internal sealed class RuleContext<TEntity, TProperty> : BuildContextBase, IRuleBuildContext<TEntity, TProperty>
{
    private readonly IRuleFailureGeneratorBuilder<TEntity, TProperty> _ruleFailureGeneratorBuilder;
    private readonly IPropertyConditionBuilder<TEntity, TProperty> _propertyConditionBuilder;
    private readonly IPropertyBuildContext _propertyContext;

    public RuleContext(
        IRuleFailureGeneratorBuilder<TEntity, TProperty> ruleFailureGeneratorBuilder,
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        IPropertyBuildContext propertyContext)
    {
        _ruleFailureGeneratorBuilder = ruleFailureGeneratorBuilder;
        _propertyConditionBuilder = propertyConditionBuilder;
        _propertyContext = propertyContext;
    }

    public Type ValidatorType => ReturnWithBuildCheck(_propertyContext.ValidatorType);

    public LambdaExpression PropertySelectorExpression =>
        ReturnWithBuildCheck(_propertyContext.PropertySelectorExpression);

    public Delegate PropertySelector => ReturnWithBuildCheck(_propertyContext.PropertySelector);

    protected override bool IsBuilt => _propertyContext.EntityBuildContext.IsValidationBuilt;

    public IMetadataProvider CreateMetadataProvider(string key)
    {
        EnsureNotBuilt();

        return _ruleFailureGeneratorBuilder.CreateMetadataProvider(key);
    }

    public void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        EnsureNotBuilt();

        _propertyConditionBuilder.AddCondition(condition);
    }

    public void ApplyIndexOnPropertyName(bool value)
    {
        EnsureNotBuilt();

        _ruleFailureGeneratorBuilder.ApplyIndexOnPropertyName(value, isDefaultMode: true);
    }

    public void SetDefaultErrorCode(string defaultErrorCode)
    {
        EnsureNotBuilt();

        _ruleFailureGeneratorBuilder.SetErrorCode(defaultErrorCode, isDefaultMode: true);
    }

    public void SetDefaultErrorDescription(string defaultErrorDescription)
    {
        EnsureNotBuilt();

        _ruleFailureGeneratorBuilder.SetErrorDescription(defaultErrorDescription, isDefaultMode: true);
    }

    public void AddErrorMetadata(string key, object? value)
    {
        EnsureNotBuilt();

        _ruleFailureGeneratorBuilder.AddErrorMetadata(key, value);
    }

    public void AddErrorMetadata(string key, Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
    {
        EnsureNotBuilt();

        _ruleFailureGeneratorBuilder.AddErrorMetadata(key, valueSelector);
    }

    public void SetMetadataLocalization(string key, Func<object?, string> localizer)
    {
        EnsureNotBuilt();

        _ruleFailureGeneratorBuilder.SetMetadataLocalization(key, localizer);
    }
}
