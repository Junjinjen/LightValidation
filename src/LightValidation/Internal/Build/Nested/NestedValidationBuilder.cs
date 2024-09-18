using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Execute.Nested;
using System;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Nested;

internal interface INestedValidationBuilder<TEntity, TProperty>
    : INestedValidatorConfiguration<TEntity, TProperty>, IPropertyValidatorBuilder<TEntity, TProperty>
{
}

internal sealed class NestedValidationBuilder<TEntity, TProperty>
    : BuilderBase, INestedValidationBuilder<TEntity, TProperty>
{
    private readonly IPropertyConditionBuilder<TEntity, TProperty> _propertyConditionBuilder;
    private readonly INestedValidationExecutorFactory _nestedValidationExecutorFactory;
    private readonly IRuleChainBuilder<TEntity, TProperty> _ruleChainBuilder;
    private readonly IScopeTracker _scopeTracker;

    private readonly Func<ValidationContext<TEntity>, IValidatorInternal<TProperty>> _validatorProvider;

    private string? _propertyName;

    public NestedValidationBuilder(
        Func<ValidationContext<TEntity>, IValidatorInternal<TProperty>> validatorProvider,
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        INestedValidationExecutorFactory nestedValidationExecutorFactory,
        IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder)
    {
        _scopeTracker = ruleChainBuilder.ValidationBuilder.RememberCurrentScope();

        _validatorProvider = validatorProvider;

        _propertyConditionBuilder = propertyConditionBuilder;
        _nestedValidationExecutorFactory = nestedValidationExecutorFactory;
        _ruleChainBuilder = ruleChainBuilder;
    }

    public IValidationBuilder<TEntity> ValidationBuilder => _ruleChainBuilder.ValidationBuilder;

    public void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        EnsureValidState();

        _propertyConditionBuilder.AddCondition(condition);
    }

    public void SetPropertyName(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        EnsureValidState();

        _propertyName = propertyName;
    }

    public void AddPropertyValidator(IPropertyValidatorBuilder<TEntity, TProperty> validatorBuilder)
    {
        EnsureValidState();

        _ruleChainBuilder.AddPropertyValidator(validatorBuilder);
    }

    public IPropertyValidator<TEntity, TProperty>? Build(IPropertyBuildContext context)
    {
        SetBuilt();

        var condition = _propertyConditionBuilder.Build();
        var propertyName = _propertyName ?? context.PropertyName;
        var metadataId = context.RegisterValidationMetadata();

        var parameters = new NestedValidationExecutorParameters<TEntity, TProperty>
        {
            ValidatorProvider = _validatorProvider,
            Condition = condition,
            PropertyName = propertyName,
            MetadataId = metadataId,
        };

        return _nestedValidationExecutorFactory.Create(parameters);
    }

    private void EnsureValidState()
    {
        EnsureNotBuilt();
        _scopeTracker.EnsureScopeUnchanged();
    }
}
