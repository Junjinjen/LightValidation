using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Constants;
using LightValidation.Internal.Build.Extensions;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Property.Context;
using LightValidation.Internal.Build.Scope;
using LightValidation.Internal.Build.Scope.Track;
using LightValidation.Internal.Build.Validation.Context;
using LightValidation.Internal.Execute.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Validation;

internal interface IValidationBuilderInternal<TEntity> : IValidationBuilder<TEntity>
{
    IValidationExecutor<TEntity> Build(Type validatorType);
}

internal sealed class ValidationBuilder<TEntity> : BuilderBase, IValidationBuilderInternal<TEntity>
{
    private const ExecutionMode DefaultExecutionMode = ExecutionMode.OnValidProperty;
    private static readonly Func<ValidationContext<TEntity>, ValueTask<bool>> DefaultCondition
        = context => ValueTask.FromResult(context.RuleSets.Contains(RuleSet.Default));

    private readonly IPropertyContextProviderCache<TEntity> _propertyContextProviderCache;
    private readonly IPropertyValidationBuilderFactory _propertyValidationBuilderFactory;
    private readonly IValidationExecutorFactory _validationExecutorFactory;
    private readonly IPropertyContextCache<TEntity> _propertyContextCache;
    private readonly INullEntityFailureBuilder _nullEntityFailureBuilder;
    private readonly IEntityContextFactory _entityContextFactory;
    private readonly IScopeBuilder<TEntity> _defaultScopeBuilder;
    private readonly IScopeTrackerFactory _scopeTrackerFactory;

    private readonly Dictionary<Type, ExecutionMode> _executionModeByAttribute = [];
    private readonly Stack<IScopeBuilder<TEntity>> _scopeBuilders = [];
    private readonly Dictionary<string, string> _propertyNames = [];
    private readonly List<IScopeBuilder<TEntity>> _roots = [];
    private ExecutionMode _defaultExecutionMode = DefaultExecutionMode;

    public ValidationBuilder(
        IConditionalScopeBuilderFactory conditionalScopeBuilderFactory,
        IPropertyContextProviderCache<TEntity> propertyContextProviderCache,
        IPropertyValidationBuilderFactory propertyValidationBuilderFactory,
        IValidationExecutorFactory validationExecutorFactory,
        IPropertyContextCache<TEntity> propertyContextCache,
        INullEntityFailureBuilder nullEntityFailureBuilder,
        IEntityContextFactory entityContextFactory,
        IScopeTrackerFactory scopeTrackerFactory)
    {
        _defaultScopeBuilder = conditionalScopeBuilderFactory.Create(DefaultCondition);
        _roots.Add(_defaultScopeBuilder);

        _propertyContextProviderCache = propertyContextProviderCache;
        _propertyValidationBuilderFactory = propertyValidationBuilderFactory;
        _validationExecutorFactory = validationExecutorFactory;
        _propertyContextCache = propertyContextCache;
        _nullEntityFailureBuilder = nullEntityFailureBuilder;
        _entityContextFactory = entityContextFactory;
        _scopeTrackerFactory = scopeTrackerFactory;
    }

    private IScopeBuilder<TEntity> CurrentScopeBuilder => _scopeBuilders.TryPeek(out var scopeBuilder)
        ? scopeBuilder
        : _defaultScopeBuilder;

    public void AllowNullEntity(bool value)
    {
        EnsureNotBuilt();

        _nullEntityFailureBuilder.AllowNullEntity(value);
    }

    public void SetNullEntityErrorCode(string errorCode)
    {
        EnsureNotBuilt();

        _nullEntityFailureBuilder.SetNullEntityErrorCode(errorCode);
    }

    public void SetNullEntityErrorDescription(string errorDescription)
    {
        EnsureNotBuilt();

        _nullEntityFailureBuilder.SetNullEntityErrorDescription(errorDescription);
    }

    public void SetPropertyName(string propertyPath, string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyPath);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        EnsureNotBuilt();

        _propertyNames[propertyPath] = propertyName;
    }

    public void SetExecutionModeForAttribute(Type attributeType, ExecutionMode mode)
    {
        attributeType.EnsureAttributeType();
        mode.EnsureDefined();
        EnsureNotBuilt();

        _executionModeByAttribute[attributeType] = mode;
    }

    public void SetDefaultExecutionMode(ExecutionMode mode)
    {
        mode.EnsureDefined();
        EnsureNotBuilt();

        _defaultExecutionMode = mode;
    }

    public void AddEntityValidator(IEntityValidatorBuilder<TEntity> validatorBuilder)
    {
        ArgumentNullException.ThrowIfNull(validatorBuilder);
        EnsureNotBuilt();

        CurrentScopeBuilder.AddEntityValidator(validatorBuilder);
    }

    public void AddPropertyValidator<TProperty>(
        Expression<Func<TEntity, TProperty>> propertySelectorExpression,
        IPropertyValidatorBuilder<TEntity, TProperty> validatorBuilder)
    {
        var propertyPath = propertySelectorExpression.EnsureValidPropertySelectorExpression();
        ArgumentNullException.ThrowIfNull(validatorBuilder);
        EnsureNotBuilt();

        var parameters = new PropertyValidationBuilderParameters<TEntity, TProperty>
        {
            PropertySelectorExpression = propertySelectorExpression,
            PropertyPath = propertyPath,
            PropertyValidatorBuilder = validatorBuilder,
            PropertyContextProviderCache = _propertyContextProviderCache,
            PropertyContextCache = _propertyContextCache,
        };

        var propertyValidationBuilder = _propertyValidationBuilderFactory.Create(parameters);

        CurrentScopeBuilder.AddEntityValidator(propertyValidationBuilder);
    }

    public void AddScope(IScopeBuilder<TEntity> scopeBuilder, bool standaloneMode, Action buildAction)
    {
        ArgumentNullException.ThrowIfNull(scopeBuilder);
        ArgumentNullException.ThrowIfNull(buildAction);
        EnsureNotBuilt();

        if (!standaloneMode)
        {
            ConnectScope(scopeBuilder);
        }

        _scopeBuilders.Push(scopeBuilder);
        buildAction.Invoke();
        _scopeBuilders.Pop();
    }

    public IScopeTracker RememberCurrentScope()
    {
        EnsureNotBuilt();

        return _scopeTrackerFactory.Create(() => CurrentScopeBuilder, CurrentScopeBuilder);
    }

    public IValidationExecutor<TEntity> Build(Type validatorType)
    {
        SetBuilt();

        var parameters = new EntityContextParameters
        {
            DefaultExecutionMode = _defaultExecutionMode,
            ValidatorType = validatorType,
            ExecutionModeByAttribute = _executionModeByAttribute,
            PropertyNames = _propertyNames,
        };

        var entityContext = _entityContextFactory.Create(parameters);
        var entityValidators = _roots.Select(x => x.Build(entityContext)).WhereNotNull().ToArray();
        var nullEntityFailure = _nullEntityFailureBuilder.Build();
        entityContext.SetBuilt();

        return _validationExecutorFactory.Create(entityValidators, nullEntityFailure, entityContext.MetadataCount);
    }

    private void ConnectScope(IScopeBuilder<TEntity> scopeBuilder)
    {
        if (_scopeBuilders.TryPeek(out var currentScopeBuilder))
        {
            currentScopeBuilder.AddEntityValidator(scopeBuilder);
            return;
        }

        _roots.Add(scopeBuilder);
    }
}
