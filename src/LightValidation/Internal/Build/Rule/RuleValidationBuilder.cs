using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Rule.Context;
using LightValidation.Internal.Build.Rule.FailureGeneration;
using LightValidation.Internal.Build.Scope;
using LightValidation.Internal.Execute.Rule;
using System;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Rule;

internal interface IRuleValidationBuilder<TEntity, TProperty, TRule>
    : IRuleConfiguration<TEntity, TProperty, TRule>, IPropertyValidatorBuilder<TEntity, TProperty>
    where TRule : notnull
{
}

internal sealed class RuleValidationBuilder<TEntity, TProperty, TRule>
    : BuilderBase, IRuleValidationBuilder<TEntity, TProperty, TRule>
    where TRule : notnull
{
    private readonly IRuleFailureGeneratorBuilder<TEntity, TProperty> _ruleFailureGeneratorBuilder;
    private readonly IPropertyConditionBuilder<TEntity, TProperty> _propertyConditionBuilder;
    private readonly IRuleValidationExecutorFactory _ruleValidationExecutorFactory;
    private readonly IDependentScopeBuilderFactory _dependentScopeBuilderFactory;
    private readonly IRuleChainBuilder<TEntity, TProperty> _ruleChainBuilder;
    private readonly IRuleBuilder<TEntity, TProperty> _ruleBuilder;
    private readonly IExecutionModeProvider _executionModeProvider;
    private readonly IRuleContextFactory _ruleContextFactory;
    private readonly IScopeTracker _scopeTracker;

    private IScopeBuilder<TEntity>? _dependentScopeBuilder;

    public RuleValidationBuilder(
        IRuleFailureGeneratorBuilder<TEntity, TProperty> ruleFailureGeneratorBuilder,
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        IRuleValidationExecutorFactory ruleValidationExecutorFactory,
        IDependentScopeBuilderFactory dependentScopeBuilderFactory,
        IRuleChainBuilder<TEntity, TProperty> ruleChainBuilder,
        IRuleBuilder<TEntity, TProperty> ruleBuilder,
        IExecutionModeProvider executionModeProvider,
        IRuleContextFactory ruleContextFactory)
    {
        _scopeTracker = ruleChainBuilder.ValidationBuilder.RememberCurrentScope();

        _ruleFailureGeneratorBuilder = ruleFailureGeneratorBuilder;
        _propertyConditionBuilder = propertyConditionBuilder;
        _ruleValidationExecutorFactory = ruleValidationExecutorFactory;
        _dependentScopeBuilderFactory = dependentScopeBuilderFactory;
        _ruleChainBuilder = ruleChainBuilder;
        _ruleBuilder = ruleBuilder;
        _executionModeProvider = executionModeProvider;
        _ruleContextFactory = ruleContextFactory;
    }

    public required TRule Rule { get; init; }

    public IValidationBuilder<TEntity> ValidationBuilder => _ruleChainBuilder.ValidationBuilder;

    public void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        EnsureValidState();

        _propertyConditionBuilder.AddCondition(condition);
    }

    public void SetExecutionMode(ExecutionMode mode)
    {
        EnsureValidState();

        _executionModeProvider.SetExecutionMode(mode);
    }

    public void SetPropertyName(string propertyName)
    {
        EnsureValidState();

        _ruleFailureGeneratorBuilder.SetPropertyName(propertyName);
    }

    public void SetErrorCode(string errorCode)
    {
        EnsureValidState();

        _ruleFailureGeneratorBuilder.SetErrorCode(errorCode);
    }

    public void SetErrorDescription(string errorDescription)
    {
        EnsureValidState();

        _ruleFailureGeneratorBuilder.SetErrorDescription(errorDescription);
    }

    public void AddErrorMetadata(string key, object? value)
    {
        EnsureValidState();

        _ruleFailureGeneratorBuilder.AddErrorMetadata(key, value);
    }

    public void AddErrorMetadata(string key, Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
    {
        EnsureValidState();

        _ruleFailureGeneratorBuilder.AddErrorMetadata(key, valueSelector);
    }

    public void SetMetadataLocalization(string key, Func<object?, string> localizer)
    {
        EnsureValidState();

        _ruleFailureGeneratorBuilder.SetMetadataLocalization(key, localizer);
    }

    public void AddDependentRules(Action buildAction)
    {
        ArgumentNullException.ThrowIfNull(buildAction);
        EnsureValidState();

        _dependentScopeBuilder ??= _dependentScopeBuilderFactory.Create<TEntity>();
        ValidationBuilder.AddScope(_dependentScopeBuilder, standaloneMode: true, buildAction);
    }

    public void AddPropertyValidator(IPropertyValidatorBuilder<TEntity, TProperty> validatorBuilder)
    {
        EnsureValidState();

        _ruleChainBuilder.AddPropertyValidator(validatorBuilder);
    }

    public IPropertyValidator<TEntity, TProperty>? Build(IPropertyBuildContext context)
    {
        SetBuilt();

        var ruleContext = _ruleContextFactory.Create(_ruleFailureGeneratorBuilder, _propertyConditionBuilder, context);
        var rule = _ruleBuilder.Build(ruleContext);

        var ruleType = Rule.GetType();
        var mode = _executionModeProvider.GetExecutionMode(
            context.ExecutionModeByAttribute, context.DefaultExecutionMode, ruleType);

        var condition = _propertyConditionBuilder.Build();
        var failureGenerator = _ruleFailureGeneratorBuilder.Build(context.PropertyName, ruleType);
        var dependentScope = _dependentScopeBuilder?.Build(context.EntityBuildContext);

        var parameters = new RuleValidationExecutorParameters<TEntity, TProperty>
        {
            ExecutionMode = mode,
            Condition = condition,
            FailureGenerator = failureGenerator,
            DependentScope = dependentScope,
            Rule = rule,
        };

        return _ruleValidationExecutorFactory.Create(parameters);
    }

    private void EnsureValidState()
    {
        EnsureNotBuilt();
        _scopeTracker.EnsureScopeUnchanged();
    }
}
