using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Rule.FailureGeneration;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Rule.Context;

internal sealed class RuleContext<TEntity, TProperty> : IRuleBuildContext<TEntity, TProperty>
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

    public Type ValidatorType => _propertyContext.ValidatorType;

    public LambdaExpression PropertySelectorExpression => _propertyContext.PropertySelectorExpression;

    public Delegate PropertySelector => _propertyContext.PropertySelector;

    public IMetadataProvider CreateMetadataProvider(string key)
    {
        return _ruleFailureGeneratorBuilder.CreateMetadataProvider(key);
    }

    public void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        _propertyConditionBuilder.AddCondition(condition);
    }

    public void SetDefaultErrorCode(string defaultErrorCode)
    {
        _ruleFailureGeneratorBuilder.SetDefaultErrorCode(defaultErrorCode);
    }

    public void SetDefaultErrorDescription(string defaultErrorDescription)
    {
        _ruleFailureGeneratorBuilder.SetDefaultErrorDescription(defaultErrorDescription);
    }

    public void AddErrorMetadata(string key, object? value)
    {
        _ruleFailureGeneratorBuilder.AddErrorMetadata(key, value);
    }

    public void AddErrorMetadata(string key, Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
    {
        _ruleFailureGeneratorBuilder.AddErrorMetadata(key, valueSelector);
    }

    public void SetMetadataLocalization(string key, Func<object?, string> localizer)
    {
        _ruleFailureGeneratorBuilder.SetMetadataLocalization(key, localizer);
    }
}
