using LightValidation.Abstractions;
using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal;
using LightValidation.Internal.Execute.Validation;
using LightValidation.Result;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LightValidation;

public abstract class ValidatorBase<TEntity> : IValidatorInternal<TEntity>
{
    private IDependencyResolver? _dependencyResolver;
    private Dictionary<Type, object?>? _services;

    public ValueTask<ValidationResult> Validate(TEntity? entity, CancellationToken cancellationToken = default)
    {
        return Validate(entity, configurationAction: null, cancellationToken);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public async ValueTask<ValidationResult> Validate(
        TEntity? entity,
        Action<IValidationOptions>? configurationAction,
        CancellationToken cancellationToken = default)
    {
        var entityContextFactory = DependencyResolver.EntityValidationContextFactory;
        var validationExecutorCache = DependencyResolver.ValidationExecutorCache;

        var brokenRules = new List<RuleFailure>();
        var validationExecutor = validationExecutorCache.Get(this);
        var validationContext = CreateValidationContext(entity, configurationAction, cancellationToken);
        using var entityContext = entityContextFactory.Create(
            brokenRules, validationExecutor.MetadataCount, validationContext);

        foreach (var mode in validationExecutor.ExecutionModes)
        {
            await validationExecutor.Validate(entityContext, mode).ConfigureAwait(false);
        }

        return new ValidationResult
        {
            ExecutedRuleSets = validationContext.RuleSets,
            BrokenRules = brokenRules,
            Cache = validationContext.Cache,
        };
    }

    public ValueTask ValidateAndThrow(TEntity? entity, CancellationToken cancellationToken = default)
    {
        return ValidateAndThrow(entity, configurationAction: null, cancellationToken);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask ValidateAndThrow(
        TEntity? entity,
        Action<IValidationOptions>? configurationAction,
        CancellationToken cancellationToken = default)
    {
        var result = await Validate(entity, configurationAction, cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
        {
            throw new ValidationException(result);
        }
    }

    public void SetDependencyResolver(IDependencyResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);

        _dependencyResolver = resolver;
    }

    ValidationContext<TEntity> IValidatorInternal<TEntity>.CreateValidationContext(
        in ValidationContextParameters<TEntity> parameters)
    {
        return new ValidationContext<TEntity>(_services, _dependencyResolver)
        {
            Entity = parameters.Entity!,
            RuleSets = parameters.RuleSets,
            Cache = parameters.Cache,
            CancellationToken = parameters.CancellationToken,
        };
    }

    IValidationExecutor<TEntity> IValidatorInternal<TEntity>.CreateValidationExecutor()
    {
        var validationBuilderFactory = DependencyResolver.ValidationBuilderFactory;
        var validationBuilder = validationBuilderFactory.Create<TEntity>();

        SetExecutionModeForComplexRule(validationBuilder);
        OnValidationBuild(validationBuilder);
        BuildValidation(validationBuilder);

        return validationBuilder.Build(GetType());
    }

    protected void SetService<TService>(TService service)
    {
        _services ??= [];
        _services[typeof(TService)] = service;
    }

    protected abstract void BuildValidation(IValidationBuilder<TEntity> builder);

    protected virtual void OnValidationBuild(IValidationBuilder<TEntity> builder)
    {
    }

    private static void SetExecutionModeForComplexRule(IValidationBuilder<TEntity> builder)
    {
        builder.SetExecutionModeForAttribute(typeof(ComplexRuleAttribute), ExecutionMode.OnValidEntity);
    }

    private ValidationContext<TEntity> CreateValidationContext(
        TEntity? entity, Action<IValidationOptions>? configurationAction, CancellationToken cancellationToken)
    {
        ValidationCache? cache = null;
        RuleSetCollection? ruleSets = null;

        if (configurationAction != null)
        {
            var validationOptionsFactory = DependencyResolver.ValidationOptionsFactory;
            var validationOptions = validationOptionsFactory.Create();
            configurationAction.Invoke(validationOptions);

            cache = validationOptions.Cache;
            ruleSets = validationOptions.RuleSets;
        }

        var parameters = new ValidationContextParameters<TEntity>
        {
            Entity = entity,
            Cache = cache ?? [],
            RuleSets = ruleSets ?? RuleSetCollection.Default,
            CancellationToken = cancellationToken,
        };

        return ((IValidatorInternal<TEntity>)this).CreateValidationContext(parameters);
    }
}
